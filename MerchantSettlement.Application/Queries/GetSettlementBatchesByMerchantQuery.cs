using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetSettlementBatchesByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantSettlementBatchDto>>;

public class GetSettlementBatchesByMerchantQueryHandler : IRequestHandler<GetSettlementBatchesByMerchantQuery, IReadOnlyList<MerchantSettlementBatchDto>>
{
    private readonly IMerchantSettlementBatchRepository _repository;

    public GetSettlementBatchesByMerchantQueryHandler(IMerchantSettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MerchantSettlementBatchDto>> Handle(GetSettlementBatchesByMerchantQuery request, CancellationToken cancellationToken)
    {
        var batches = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);
        return batches.Select(MapToDto).ToList();
    }

    private static MerchantSettlementBatchDto MapToDto(MerchantSettlementBatch batch)
    {
        return new MerchantSettlementBatchDto
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