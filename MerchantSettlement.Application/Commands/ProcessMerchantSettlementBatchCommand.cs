using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record ProcessMerchantSettlementBatchCommand(Guid BatchId, string ProcessedBy) : IRequest<Result<MerchantSettlementBatchDto>>;

public class ProcessSettlementBatchCommandHandler : IRequestHandler<ProcessMerchantSettlementBatchCommand, Result<MerchantSettlementBatchDto>>
{
    private readonly IMerchantSettlementBatchRepository _repository;

    public ProcessSettlementBatchCommandHandler(IMerchantSettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantSettlementBatchDto>> Handle(ProcessMerchantSettlementBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await _repository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<MerchantSettlementBatchDto>("Batch bulunamadı");

        var startResult = batch.StartProcessing(request.ProcessedBy);
        if (startResult.IsFailure)
            return Result.Failure<MerchantSettlementBatchDto>(startResult.Error);

        // İşleme simülasyonu - gerçek senaryoda burada işlem yapılır
        var completeResult = batch.Complete(request.ProcessedBy);
        if (completeResult.IsFailure)
            return Result.Failure<MerchantSettlementBatchDto>(completeResult.Error);

        _repository.Update(batch);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(batch);
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