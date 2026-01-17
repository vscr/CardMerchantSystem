using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Commands;

public record SchedulePayoutCommand(Guid PayoutId, DateTime ScheduledDate, string OperatorUsername) : IRequest<Result<MerchantPayoutDto>>;

public class SchedulePayoutCommandHandler : IRequestHandler<SchedulePayoutCommand, Result<MerchantPayoutDto>>
{
    private readonly IMerchantPayoutRepository _repository;

    public SchedulePayoutCommandHandler(IMerchantPayoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantPayoutDto>> Handle(SchedulePayoutCommand request, CancellationToken cancellationToken)
    {
        var payout = await _repository.GetByIdAsync(request.PayoutId, cancellationToken);
        if (payout is null)
            return Result.Failure<MerchantPayoutDto>("Ödeme bulunamadı");

        var result = payout.Schedule(request.ScheduledDate, request.OperatorUsername);
        if (result.IsFailure)
            return Result.Failure<MerchantPayoutDto>(result.Error);

        _repository.Update(payout);
        await _repository.SaveChangesAsync(cancellationToken);

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