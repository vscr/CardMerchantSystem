using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record AddSettlementDetailsCommand(Guid BatchId, List<AddSettlementDetailDto> Details) : IRequest<Result<SettlementBatchDto>>;

public class AddSettlementDetailsCommandHandler : IRequestHandler<AddSettlementDetailsCommand, Result<SettlementBatchDto>>
{
    private readonly ISettlementBatchRepository _repository;

    public AddSettlementDetailsCommandHandler(ISettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SettlementBatchDto>> Handle(AddSettlementDetailsCommand request, CancellationToken cancellationToken)
    {
        var batch = await _repository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<SettlementBatchDto>("Batch bulunamadı");

        var details = request.Details.Select(d => SettlementDetail.Create(
            batch.Id,
            d.TransactionId,
            d.TransactionNumber,
            d.TransactionType,
            d.TransactionDate,
            d.CardNumberMasked,
            d.CardBrand,
            d.TerminalId,
            d.Amount,
            d.CommissionRate,
            d.CommissionAmount,
            d.FeeAmount,
            d.InstallmentCount,
            d.OriginalTransactionId,
            d.AuthorizationCode,
            d.ReferenceNumber
        )).ToList();

        var addResult = batch.AddDetails(details);
        if (addResult.IsFailure)
            return Result.Failure<SettlementBatchDto>(addResult.Error);

        _repository.Update(batch);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(batch);
    }

    private static SettlementBatchDto MapToDto(SettlementBatch batch)
    {
        return new SettlementBatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            MerchantId = batch.MerchantId,
            MerchantName = batch.MerchantName,
            PeriodStart = batch.PeriodStart,
            PeriodEnd = batch.PeriodEnd,
            SettlementType = batch.SettlementType.Name,
            SettlementTypeDisplayName = batch.SettlementType.DisplayName,
            Status = batch.Status.Name,
            StatusDisplayName = batch.Status.DisplayName,
            TotalSalesAmount = batch.TotalSalesAmount,
            TotalRefundAmount = batch.TotalRefundAmount,
            TotalChargebackAmount = batch.TotalChargebackAmount,
            GrossAmount = batch.GrossAmount,
            TotalCommission = batch.TotalCommission,
            TotalFee = batch.TotalFee,
            NetAmount = batch.NetAmount,
            Currency = batch.Currency,
            SalesCount = batch.SalesCount,
            RefundCount = batch.RefundCount,
            ChargebackCount = batch.ChargebackCount,
            TotalTransactionCount = batch.TotalTransactionCount,
            ProcessedAt = batch.ProcessedAt,
            ProcessedBy = batch.ProcessedBy,
            FailureReason = batch.FailureReason,
            CreatedAt = batch.CreatedAt
        };
    }
}