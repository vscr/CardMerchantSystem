using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Commands;

public class ProcessTransactionCommandHandler
    : IRequestHandler<ProcessTransactionCommand, Result<TransactionResultDto>>
{
    private readonly ITransactionRepository _repository;
    private readonly ILimitService _limitService;
    private readonly IFraudService _fraudService;

    public ProcessTransactionCommandHandler(
        ITransactionRepository repository,
        ILimitService limitService,
        IFraudService fraudService)
    {
        _repository = repository;
        _limitService = limitService;
        _fraudService = fraudService;
    }

    public async Task<Result<TransactionResultDto>> Handle(
        ProcessTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // 1. Transaction Type bul
        var transactionType = TransactionType.FromId<TransactionType>(dto.TransactionTypeId);
        if (transactionType == null)
            return Result.Failure<TransactionResultDto>("Geçersiz işlem tipi", ErrorCodes.ValidationError);

        // 2. Amount oluştur
        var amountResult = TransactionAmount.Create(dto.Amount, dto.Currency);
        if (amountResult.IsFailure)
            return Result.Failure<TransactionResultDto>(amountResult.Error!, amountResult.ErrorCode);

        // 3. Transaction oluştur
        var transactionResult = TransactionAggregate.Create(
            transactionType,
            amountResult.Value!,
            dto.CardNumberMasked,
            dto.CardNumberEncrypted,
            dto.MerchantId,
            dto.MerchantCode,
            dto.TerminalId,
            dto.TerminalCode,
            dto.OriginalTransactionId);

        if (transactionResult.IsFailure)
            return Result.Failure<TransactionResultDto>(transactionResult.Error!, transactionResult.ErrorCode);

        var transaction = transactionResult.Value!;

        // 4. Fraud kontrolü
        var fraudRequest = new FraudCheckRequest
        {
            CardNumberMasked = dto.CardNumberMasked,
            Amount = dto.Amount,
            MerchantCode = dto.MerchantCode,
            TerminalCode = dto.TerminalCode,
            TransactionTime = DateTime.UtcNow
        };

        var fraudResult = await _fraudService.CheckFraudAsync(fraudRequest, cancellationToken);
        if (fraudResult.IsSuccess)
        {
            transaction.SetFraudCheckResult(fraudResult.Value!.Result, fraudResult.Value.Score);

            if (fraudResult.Value.Result == FraudCheckResult.Reject)
            {
                await _repository.AddAsync(transaction, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);

                return CreateResult(transaction, "05", "Fraud şüphesi - İşlem reddedildi");
            }
        }

        // 5. Limit kontrolü (sadece limit düşen işlemler için)
        if (transactionType.DecreasesLimit)
        {
            var limitResult = await _limitService.CheckLimitAsync(
                dto.CardNumberMasked, dto.Amount, cancellationToken);

            if (limitResult.IsFailure)
            {
                transaction.Decline(DeclineReason.InsufficientLimit, limitResult.Error);
                await _repository.AddAsync(transaction, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);

                return CreateResult(transaction, "51", limitResult.Error!);
            }

            // Limit rezerve et
            var reserveResult = await _limitService.ReserveLimitAsync(
                dto.CardNumberMasked, dto.Amount, transaction.Id.ToString(), cancellationToken);

            if (reserveResult.IsFailure)
            {
                transaction.Decline(DeclineReason.InsufficientLimit, reserveResult.Error);
                await _repository.AddAsync(transaction, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);

                return CreateResult(transaction, "51", reserveResult.Error!);
            }
        }

        // 6. İşlemi onayla
        var approveResult = transaction.Approve();
        if (approveResult.IsFailure)
        {
            // Limit geri al
            if (transactionType.DecreasesLimit)
            {
                await _limitService.ReleaseLimitAsync(
                    dto.CardNumberMasked, dto.Amount, transaction.Id.ToString(), cancellationToken);
            }

            return Result.Failure<TransactionResultDto>(approveResult.Error!, approveResult.ErrorCode);
        }

        // 7. Limit kullanımını onayla
        if (transactionType.DecreasesLimit)
        {
            await _limitService.CommitLimitAsync(
                dto.CardNumberMasked, dto.Amount, transaction.Id.ToString(), cancellationToken);
        }

        // 8. Limit iade (iade/iptal işlemleri için)
        if (transactionType.IncreasesLimit)
        {
            await _limitService.RefundLimitAsync(
                dto.CardNumberMasked, dto.Amount, transaction.Id.ToString(), cancellationToken);
        }

        // 9. Kaydet
        await _repository.AddAsync(transaction, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreateResult(transaction, "00", "İşlem onaylandı");
    }

    private static TransactionResultDto CreateResult(TransactionAggregate transaction, string responseCode, string responseMessage)
    {
        return new TransactionResultDto
        {
            IsApproved = transaction.Status == TransactionStatus.Approved,
            ReferenceNumber = transaction.ReferenceNumber.Value,
            AuthorizationCode = transaction.AuthorizationCode?.Value,
            ResponseCode = responseCode,
            ResponseMessage = responseMessage,
            Amount = transaction.Amount.Amount,
            Currency = transaction.Amount.Currency,
            TransactionTime = transaction.CreatedAt
        };
    }
}