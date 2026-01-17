using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetUnresolvedReconciliationsQuery() : IRequest<IReadOnlyList<SettlementReconciliationDto>>;

public class GetUnresolvedReconciliationsQueryHandler : IRequestHandler<GetUnresolvedReconciliationsQuery, IReadOnlyList<SettlementReconciliationDto>>
{
    private readonly ISettlementReconciliationRepository _repository;

    public GetUnresolvedReconciliationsQueryHandler(ISettlementReconciliationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SettlementReconciliationDto>> Handle(GetUnresolvedReconciliationsQuery request, CancellationToken cancellationToken)
    {
        var reconciliations = await _repository.GetUnresolvedAsync(cancellationToken);
        return reconciliations.Select(MapToDto).ToList();
    }

    private static SettlementReconciliationDto MapToDto(SettlementReconciliation reconciliation)
    {
        return new SettlementReconciliationDto
        {
            Id = reconciliation.Id,
            ReconciliationNumber = reconciliation.ReconciliationNumber,
            SettlementBatchId = reconciliation.SettlementBatchId,
            MerchantId = reconciliation.MerchantId,
            MerchantName = reconciliation.MerchantName,
            ReconciliationDate = reconciliation.ReconciliationDate,
            SystemGrossAmount = reconciliation.SystemGrossAmount,
            SystemCommission = reconciliation.SystemCommission,
            SystemNetAmount = reconciliation.SystemNetAmount,
            SystemTransactionCount = reconciliation.SystemTransactionCount,
            ReportedGrossAmount = reconciliation.ReportedGrossAmount,
            ReportedCommission = reconciliation.ReportedCommission,
            ReportedNetAmount = reconciliation.ReportedNetAmount,
            ReportedTransactionCount = reconciliation.ReportedTransactionCount,
            GrossAmountDifference = reconciliation.GrossAmountDifference,
            CommissionDifference = reconciliation.CommissionDifference,
            NetAmountDifference = reconciliation.NetAmountDifference,
            TransactionCountDifference = reconciliation.TransactionCountDifference,
            Status = reconciliation.Status.Name,
            StatusDisplayName = reconciliation.Status.DisplayName,
            ResolutionNotes = reconciliation.ResolutionNotes,
            ResolvedBy = reconciliation.ResolvedBy,
            ResolvedAt = reconciliation.ResolvedAt,
            CreatedAt = reconciliation.CreatedAt
        };
    }
}