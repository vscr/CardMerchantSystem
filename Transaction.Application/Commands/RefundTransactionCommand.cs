using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;

namespace Transaction.Application.Commands;

/// <summary>
/// İade işlemi komutu
/// </summary>
public record RefundTransactionCommand(
    Guid OriginalTransactionId,
    decimal Amount,
    string OperatorUsername
) : IRequest<Result<TransactionResultDto>>;
public class RefundTransactionCommandHandler
    : IRequestHandler<RefundTransactionCommand, Result<TransactionResultDto>>
{
    private readonly ITransactionRepository _repository;
    private readonly ILimitService _limitService;

    public RefundTransactionCommandHandler(
        ITransactionRepository repository,
        ILimitService limitService)
    {
        _repository = repository;
        _limitService = limitService;
    }

    public async Task<Result<TransactionResultDto>> Handle(
        RefundTransactionCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Orijinal işlemi bul
        var originalTransaction = await _repository.GetByIdAsync(request.OriginalTransactionId, cancellationToken);
        if (originalTransaction == null)
            return Result.Failure<TransactionResultDto>("Orijinal işlem bulunamadı", ErrorCodes.TransactionFailed);

        // 2. Orijinal işlem onaylı mı kontrol et
        if (originalTransaction.Status != TransactionStatus.Approved &&
            originalTransaction.Status != TransactionStatus.Settled)
            return Result.Failure<TransactionResultDto>("Sadece onaylı veya takas edilmiş işlemler iade edilebilir");

        // 3. Mevcut iade toplamını kontrol et
        var totalRefundedAmount = await _repository.GetTotalRefundedAmountAsync(
            request.OriginalTransactionId,
            cancellationToken);

        var remainingRefundableAmount = originalTransaction.Amount.Amount - totalRefundedAmount;

        // 3a. Tüm tutar zaten iade edilmiş mi?
        if (remainingRefundableAmount <= 0)
            return Result.Failure<TransactionResultDto>(
                "Bu işlemin tamamı zaten iade edilmiş",
                ErrorCodes.RefundAlreadyProcessed);

        // 3b. Talep edilen tutar, kalan iade edilebilir tutarı aşıyor mu?
        if (request.Amount > remainingRefundableAmount)
            return Result.Failure<TransactionResultDto>(
                $"İade edilebilir tutar: {remainingRefundableAmount:N2} {originalTransaction.Amount.Currency}. " +
                $"Daha önce {totalRefundedAmount:N2} {originalTransaction.Amount.Currency} iade edilmiş.",
                ErrorCodes.RefundAmountExceeded);

        // 4. Amount oluştur
        var amountResult = TransactionAmount.Create(request.Amount, originalTransaction.Amount.Currency);
        if (amountResult.IsFailure)
            return Result.Failure<TransactionResultDto>(amountResult.Error!, amountResult.ErrorCode);

        // 5. İade işlemi oluştur
        var refundResult = TransactionAggregate.Create(
            TransactionType.Refund,
            amountResult.Value!,
            originalTransaction.CardNumberMasked,
            originalTransaction.CardNumberEncrypted,
            originalTransaction.MerchantId,
            originalTransaction.MerchantCode,
            originalTransaction.TerminalId,
            originalTransaction.TerminalCode,
            originalTransaction.Id);

        if (refundResult.IsFailure)
            return Result.Failure<TransactionResultDto>(refundResult.Error!, refundResult.ErrorCode);

        var refundTransaction = refundResult.Value!;

        // 6. İşlemi onayla
        refundTransaction.Approve();

        // 7. Limit iade et
        await _limitService.RefundLimitAsync(
            originalTransaction.CardNumberMasked,
            request.Amount,
            refundTransaction.Id.ToString(),
            cancellationToken);

        // 8. Kaydet
        await _repository.AddAsync(refundTransaction, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 9. Response oluştur
        var isFullRefund = (totalRefundedAmount + request.Amount) >= originalTransaction.Amount.Amount;
        var responseMessage = isFullRefund
            ? "Tam iade işlemi onaylandı"
            : $"Kısmi iade işlemi onaylandı. Kalan iade edilebilir tutar: {remainingRefundableAmount - request.Amount:N2} {originalTransaction.Amount.Currency}";

        return new TransactionResultDto
        {
            IsApproved = true,
            ReferenceNumber = refundTransaction.ReferenceNumber.Value,
            AuthorizationCode = refundTransaction.AuthorizationCode?.Value,
            ResponseCode = "00",
            ResponseMessage = responseMessage,
            Amount = refundTransaction.Amount.Amount,
            Currency = refundTransaction.Amount.Currency,
            TransactionTime = refundTransaction.CreatedAt
        };
    }
}