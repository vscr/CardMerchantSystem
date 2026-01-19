using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record CreateMerchantPayoutCommand(CreateMerchantPayoutDto Dto) : IRequest<Result<MerchantPayoutDto>>;

public class CreateMerchantPayoutCommandHandler : IRequestHandler<CreateMerchantPayoutCommand, Result<MerchantPayoutDto>>
{
    private readonly IMerchantPayoutRepository _payoutRepository;
    private readonly IMerchantSettlementBatchRepository _batchRepository;

    public CreateMerchantPayoutCommandHandler(
        IMerchantPayoutRepository payoutRepository,
        IMerchantSettlementBatchRepository batchRepository)
    {
        _payoutRepository = payoutRepository;
        _batchRepository = batchRepository;
    }

    public async Task<Result<MerchantPayoutDto>> Handle(CreateMerchantPayoutCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Batch'leri getir ve toplamları hesapla
        decimal grossAmount = 0;
        decimal totalCommission = 0;
        decimal totalFee = 0;

        foreach (var batchId in dto.SettlementBatchIds)
        {
            var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
            if (batch is null)
                return Result.Failure<MerchantPayoutDto>($"Batch bulunamadı: {batchId}");

            if (batch.MerchantId != dto.MerchantId)
                return Result.Failure<MerchantPayoutDto>($"Batch farklı merchant'a ait: {batchId}");

            grossAmount += batch.GrossAmount;
            totalCommission += batch.TotalCommission;
            totalFee += batch.TotalFee;
        }

        var withholdingTax = grossAmount * dto.WithholdingTaxRate / 100;

        var payoutResult = MerchantPayout.Create(
            dto.MerchantId,
            dto.MerchantName,
            dto.BankCode,
            dto.BankName,
            dto.Iban,
            dto.PeriodStart,
            dto.PeriodEnd,
            grossAmount,
            totalCommission,
            totalFee,
            withholdingTax);

        if (payoutResult.IsFailure)
            return Result.Failure<MerchantPayoutDto>(payoutResult.Error);

        var payout = payoutResult.Value!;

        foreach (var batchId in dto.SettlementBatchIds)
        {
            payout.AddSettlementBatch(batchId);
        }

        await _payoutRepository.AddAsync(payout, cancellationToken);
        await _payoutRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(payout);
    }

    private static MerchantPayoutDto MapToDto(MerchantPayout payout)
    {
        return new MerchantPayoutDto
        {
            Id = payout.Id,
            PayoutNumber = payout.PayoutNumber,
            MerchantId = payout.MerchantId,
            MerchantName = payout.MerchantName,
            BankCode = payout.BankCode,
            BankName = payout.BankName,
            Iban = payout.Iban,
            PeriodStart = payout.PeriodStart,
            PeriodEnd = payout.PeriodEnd,
            GrossAmount = payout.GrossAmount,
            TotalCommission = payout.TotalCommission,
            TotalFee = payout.TotalFee,
            WithholdingTax = payout.WithholdingTax,
            NetAmount = payout.NetAmount,
            Currency = payout.Currency,
            Status = payout.Status.Name,
            StatusDisplayName = payout.Status.DisplayName,
            ScheduledDate = payout.ScheduledDate,
            PaidAt = payout.PaidAt,
            BankReferenceNumber = payout.BankReferenceNumber,
            HoldReason = payout.HoldReason,
            FailureReason = payout.FailureReason,
            SettlementBatchIds = payout.SettlementBatchIds.ToList(),
            CreatedAt = payout.CreatedAt
        };
    }
}