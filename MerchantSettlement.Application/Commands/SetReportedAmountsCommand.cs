using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record SetReportedAmountsCommand(SetReportedAmountsDto Dto, string OperatorUsername) : IRequest<Result<SettlementReconciliationDto>>;

public class SetReportedAmountsCommandHandler : IRequestHandler<SetReportedAmountsCommand, Result<SettlementReconciliationDto>>
{
    private readonly IMerchantSettlementReconciliationRepository _repository;

    public SetReportedAmountsCommandHandler(IMerchantSettlementReconciliationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SettlementReconciliationDto>> Handle(SetReportedAmountsCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reconciliation = await _repository.GetByIdAsync(dto.ReconciliationId, cancellationToken);
        if (reconciliation is null)
            return Result.Failure<SettlementReconciliationDto>("Mutabakat bulunamadı");

        var result = reconciliation.SetReportedAmounts(
            dto.ReportedGrossAmount,
            dto.ReportedCommission,
            dto.ReportedNetAmount,
            dto.ReportedTransactionCount,
            request.OperatorUsername);

        if (result.IsFailure)
            return Result.Failure<SettlementReconciliationDto>(result.Error);

        _repository.Update(reconciliation);
        await _repository.SaveChangesAsync(cancellationToken);

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
}