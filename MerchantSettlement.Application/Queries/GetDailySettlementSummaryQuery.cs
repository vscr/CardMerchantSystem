using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetDailySettlementSummaryQuery(DateTime Date) : IRequest<DailySettlementSummaryDto?>;

public class GetDailySettlementSummaryQueryHandler : IRequestHandler<GetDailySettlementSummaryQuery, DailySettlementSummaryDto?>
{
    private readonly IMerchantDailySettlementSummaryRepository _repository;

    public GetDailySettlementSummaryQueryHandler(IMerchantDailySettlementSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<DailySettlementSummaryDto?> Handle(GetDailySettlementSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _repository.GetByDateAsync(request.Date, cancellationToken);
        if (summary is null)
            return null;

        return MapToDto(summary);
    }

    private static DailySettlementSummaryDto MapToDto(MerchantDailySettlementSummary summary)
    {
        return new DailySettlementSummaryDto
        {
            Id = summary.Id,
            SettlementDate = summary.SettlementDate,
            TotalMerchantCount = summary.TotalMerchantCount,
            TotalBatchCount = summary.TotalBatchCount,
            TotalTransactionCount = summary.TotalTransactionCount,
            TotalSalesAmount = summary.TotalSalesAmount,
            TotalRefundAmount = summary.TotalRefundAmount,
            TotalChargebackAmount = summary.TotalChargebackAmount,
            TotalGrossAmount = summary.TotalGrossAmount,
            TotalCommission = summary.TotalCommission,
            TotalFee = summary.TotalFee,
            TotalNetAmount = summary.TotalNetAmount,
            Currency = summary.Currency,
            SalesCount = summary.SalesCount,
            RefundCount = summary.RefundCount,
            ChargebackCount = summary.ChargebackCount,
            CompletedBatchCount = summary.CompletedBatchCount,
            FailedBatchCount = summary.FailedBatchCount,
            PendingBatchCount = summary.PendingBatchCount,
            IsFinalized = summary.IsFinalized,
            FinalizedAt = summary.FinalizedAt,
            FinalizedBy = summary.FinalizedBy,
            CreatedAt = summary.CreatedAt
        };
    }
}