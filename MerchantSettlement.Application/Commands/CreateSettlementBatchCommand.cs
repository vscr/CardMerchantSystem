using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record CreateSettlementBatchCommand(CreateSettlementBatchDto Dto) : IRequest<Result<SettlementBatchDto>>;

public class CreateSettlementBatchCommandHandler : IRequestHandler<CreateSettlementBatchCommand, Result<SettlementBatchDto>>
{
    private readonly IMerchantSettlementBatchRepository _repository;

    public CreateSettlementBatchCommandHandler(IMerchantSettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SettlementBatchDto>> Handle(CreateSettlementBatchCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var settlementType = Enumeration.FromId<SettlementType>(dto.SettlementTypeId);
        if (settlementType is null)
            return Result.Failure<SettlementBatchDto>("Geçersiz takas tipi");

        var batchResult = MerchantSettlementBatch.Create(
            dto.MerchantId,
            dto.MerchantName,
            dto.PeriodStart,
            dto.PeriodEnd,
            settlementType);

        if (batchResult.IsFailure)
            return Result.Failure<SettlementBatchDto>(batchResult.Error);

        var batch = batchResult.Value!;
        await _repository.AddAsync(batch, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(batch);
    }

    private static SettlementBatchDto MapToDto(MerchantSettlementBatch batch)
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