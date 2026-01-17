using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetSettlementBatchesByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<SettlementBatchDto>>;

public class GetSettlementBatchesByMerchantQueryHandler : IRequestHandler<GetSettlementBatchesByMerchantQuery, IReadOnlyList<SettlementBatchDto>>
{
    private readonly ISettlementBatchRepository _repository;

    public GetSettlementBatchesByMerchantQueryHandler(ISettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SettlementBatchDto>> Handle(GetSettlementBatchesByMerchantQuery request, CancellationToken cancellationToken)
    {
        var batches = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);
        return batches.Select(MapToDto).ToList();
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