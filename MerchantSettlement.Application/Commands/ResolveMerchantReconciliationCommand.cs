using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record ResolveMerchantReconciliationCommand(Guid ReconciliationId, string Notes, string OperatorUsername) : IRequest<Result<MerchantSettlementReconciliationDto>>;

public class ResolveReconciliationCommandHandler : IRequestHandler<ResolveMerchantReconciliationCommand, Result<MerchantSettlementReconciliationDto>>
{
    private readonly IMerchantSettlementReconciliationRepository _repository;

    public ResolveReconciliationCommandHandler(IMerchantSettlementReconciliationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantSettlementReconciliationDto>> Handle(ResolveMerchantReconciliationCommand request, CancellationToken cancellationToken)
    {
        var reconciliation = await _repository.GetByIdAsync(request.ReconciliationId, cancellationToken);
        if (reconciliation is null)
            return Result.Failure<MerchantSettlementReconciliationDto>("Mutabakat bulunamadı");

        var result = reconciliation.Resolve(request.Notes, request.OperatorUsername);
        if (result.IsFailure)
            return Result.Failure<MerchantSettlementReconciliationDto>(result.Error);

        _repository.Update(reconciliation);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(reconciliation);
    }

    private static MerchantSettlementReconciliationDto MapToDto(MerchantReconciliation reconciliation)
    {
        return new MerchantSettlementReconciliationDto
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