using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record CreateMerchantSettlementBatchCommand(CreateMerchantSettlementBatchDto Dto) : IRequest<Result<MerchantSettlementBatchDto>>;

public class CreateSettlementBatchCommandHandler : IRequestHandler<CreateMerchantSettlementBatchCommand, Result<MerchantSettlementBatchDto>>
{
    private readonly IMerchantSettlementBatchRepository _repository;

    public CreateSettlementBatchCommandHandler(IMerchantSettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantSettlementBatchDto>> Handle(CreateMerchantSettlementBatchCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var settlementType = Enumeration.FromId<SettlementType>(dto.SettlementTypeId);
        if (settlementType is null)
            return Result.Failure<MerchantSettlementBatchDto>("Geçersiz takas tipi");

        var batchResult = MerchantSettlementBatch.Create(
            dto.MerchantId,
            dto.MerchantName,
            dto.PeriodStart,
            dto.PeriodEnd,
            settlementType);

        if (batchResult.IsFailure)
            return Result.Failure<MerchantSettlementBatchDto>(batchResult.Error);

        var batch = batchResult.Value!;
        await _repository.AddAsync(batch, cancellationToken);
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