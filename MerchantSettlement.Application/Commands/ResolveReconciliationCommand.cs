using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record ResolveReconciliationCommand(Guid ReconciliationId, string Notes, string OperatorUsername) : IRequest<Result<SettlementReconciliationDto>>;

public class ResolveReconciliationCommandHandler : IRequestHandler<ResolveReconciliationCommand, Result<SettlementReconciliationDto>>
{
    private readonly ISettlementReconciliationRepository _repository;

    public ResolveReconciliationCommandHandler(ISettlementReconciliationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SettlementReconciliationDto>> Handle(ResolveReconciliationCommand request, CancellationToken cancellationToken)
    {
        var reconciliation = await _repository.GetByIdAsync(request.ReconciliationId, cancellationToken);
        if (reconciliation is null)
            return Result.Failure<SettlementReconciliationDto>("Mutabakat bulunamadı");

        var result = reconciliation.Resolve(request.Notes, request.OperatorUsername);
        if (result.IsFailure)
            return Result.Failure<SettlementReconciliationDto>(result.Error);

        _repository.Update(reconciliation);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(reconciliation);
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