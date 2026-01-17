using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetSettlementBatchByIdQuery(Guid Id, bool IncludeDetails = false) : IRequest<SettlementBatchDto?>;

public class GetSettlementBatchByIdQueryHandler : IRequestHandler<GetSettlementBatchByIdQuery, SettlementBatchDto?>
{
    private readonly ISettlementBatchRepository _repository;

    public GetSettlementBatchByIdQueryHandler(ISettlementBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<SettlementBatchDto?> Handle(GetSettlementBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = request.IncludeDetails
            ? await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            : await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (batch is null)
            return null;

        if (request.IncludeDetails)
            return MapToDtoWithDetails(batch);

        return MapToDto(batch);
    }

    private static SettlementBatchDto MapToDto(SettlementBatch batch)
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

    private static SettlementBatchWithDetailsDto MapToDtoWithDetails(SettlementBatch batch)
    {
        return new SettlementBatchWithDetailsDto
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
            CreatedAt = batch.CreatedAt,
            Details = batch.Details.Select(d => new SettlementDetailDto
            {
                Id = d.Id,
                SettlementBatchId = d.SettlementBatchId,
                TransactionId = d.TransactionId,
                TransactionNumber = d.TransactionNumber,
                TransactionType = d.TransactionType,
                TransactionDate = d.TransactionDate,
                CardNumberMasked = d.CardNumberMasked,
                CardBrand = d.CardBrand,
                TerminalId = d.TerminalId,
                Amount = d.Amount,
                CommissionRate = d.CommissionRate,
                CommissionAmount = d.CommissionAmount,
                FeeAmount = d.FeeAmount,
                NetAmount = d.NetAmount,
                Currency = d.Currency,
                InstallmentCount = d.InstallmentCount,
                OriginalTransactionId = d.OriginalTransactionId,
                AuthorizationCode = d.AuthorizationCode,
                ReferenceNumber = d.ReferenceNumber
            }).ToList()
        };
    }
}