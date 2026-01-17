using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record CreateReconciliationCommand(Guid BatchId) : IRequest<Result<SettlementReconciliationDto>>;

public class CreateReconciliationCommandHandler : IRequestHandler<CreateReconciliationCommand, Result<SettlementReconciliationDto>>
{
    private readonly ISettlementReconciliationRepository _reconciliationRepository;
    private readonly ISettlementBatchRepository _batchRepository;

    public CreateReconciliationCommandHandler(
        ISettlementReconciliationRepository reconciliationRepository,
        ISettlementBatchRepository batchRepository)
    {
        _reconciliationRepository = reconciliationRepository;
        _batchRepository = batchRepository;
    }

    public async Task<Result<SettlementReconciliationDto>> Handle(CreateReconciliationCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<SettlementReconciliationDto>("Batch bulunamadı");

        // Aynı batch için mevcut mutabakat var mı kontrol et
        var existingReconciliation = await _reconciliationRepository.GetByBatchIdAsync(request.BatchId, cancellationToken);
        if (existingReconciliation is not null)
            return Result.Failure<SettlementReconciliationDto>("Bu batch için mutabakat zaten mevcut");

        var reconciliationResult = SettlementReconciliation.Create(
            batch.Id,
            batch.MerchantId,
            batch.MerchantName,
            DateTime.UtcNow,
            batch.GrossAmount,
            batch.TotalCommission,
            batch.NetAmount,
            batch.TotalTransactionCount);

        if (reconciliationResult.IsFailure)
            return Result.Failure<SettlementReconciliationDto>(reconciliationResult.Error);

        var reconciliation = reconciliationResult.Value!;
        await _reconciliationRepository.AddAsync(reconciliation, cancellationToken);
        await _reconciliationRepository.SaveChangesAsync(cancellationToken);

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