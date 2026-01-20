using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetPendingPayoutsQuery() : IRequest<IReadOnlyList<MerchantPayoutDto>>;

public class GetPendingPayoutsQueryHandler : IRequestHandler<GetPendingPayoutsQuery, IReadOnlyList<MerchantPayoutDto>>
{
    private readonly IMerchantPayoutRepository _repository;

    public GetPendingPayoutsQueryHandler(IMerchantPayoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MerchantPayoutDto>> Handle(GetPendingPayoutsQuery request, CancellationToken cancellationToken)
    {
        var payouts = await _repository.GetPendingPayoutsAsync(cancellationToken);
        return payouts.Select(MapToDto).ToList();
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