using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetReconciliationByIdQuery(Guid Id, bool IncludeMismatches = false) : IRequest<SettlementReconciliationDto?>;

public class GetReconciliationByIdQueryHandler : IRequestHandler<GetReconciliationByIdQuery, SettlementReconciliationDto?>
{
    private readonly IMerchantSettlementReconciliationRepository _repository;

    public GetReconciliationByIdQueryHandler(IMerchantSettlementReconciliationRepository repository)
    {
        _repository = repository;
    }

    public async Task<SettlementReconciliationDto?> Handle(GetReconciliationByIdQuery request, CancellationToken cancellationToken)
    {
        var reconciliation = request.IncludeMismatches
            ? await _repository.GetByIdWithMismatchesAsync(request.Id, cancellationToken)
            : await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (reconciliation is null)
            return null;

        if (request.IncludeMismatches)
            return MapToDtoWithMismatches(reconciliation);

        return MapToDto(reconciliation);
    }

    private static SettlementReconciliationDto MapToDto(MerchantReconciliation reconciliation)
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

    private static SettlementReconciliationWithMismatchesDto MapToDtoWithMismatches(MerchantReconciliation reconciliation)
    {
        return new SettlementReconciliationWithMismatchesDto
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
            CreatedAt = reconciliation.CreatedAt,
            Mismatches = reconciliation.Mismatches.Select(m => new ReconciliationMismatchDto
            {
                Id = m.Id,
                ReconciliationId = m.ReconciliationId,
                TransactionId = m.TransactionId,
                TransactionNumber = m.TransactionNumber,
                TransactionDate = m.TransactionDate,
                MismatchType = m.MismatchType,
                SystemAmount = m.SystemAmount,
                ReportedAmount = m.ReportedAmount,
                AmountDifference = m.AmountDifference,
                Description = m.Description,
                IsResolved = m.IsResolved,
                ResolutionNotes = m.ResolutionNotes,
                ResolvedAt = m.ResolvedAt
            }).ToList()
        };
    }
}