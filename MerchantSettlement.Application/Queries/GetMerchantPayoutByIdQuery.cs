using MediatR;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;

namespace MerchantSettlement.Application.Queries;

public record GetMerchantPayoutByIdQuery(Guid Id) : IRequest<MerchantPayoutDto?>;

public class GetMerchantPayoutByIdQueryHandler : IRequestHandler<GetMerchantPayoutByIdQuery, MerchantPayoutDto?>
{
    private readonly IMerchantPayoutRepository _repository;

    public GetMerchantPayoutByIdQueryHandler(IMerchantPayoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<MerchantPayoutDto?> Handle(GetMerchantPayoutByIdQuery request, CancellationToken cancellationToken)
    {
        var payout = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (payout is null)
            return null;

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