using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Queries;

public record GetFeeAccrualsByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<FeeAccrualDto>>;
public class GetFeeAccrualsByMerchantQueryHandler
    : IRequestHandler<GetFeeAccrualsByMerchantQuery, IReadOnlyList<FeeAccrualDto>>
{
    private readonly IFeeAccrualRepository _repository;

    public GetFeeAccrualsByMerchantQueryHandler(IFeeAccrualRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<FeeAccrualDto>> Handle(
        GetFeeAccrualsByMerchantQuery request,
        CancellationToken cancellationToken)
    {
        var accruals = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

        return accruals.Select(MapToDto).ToList();
    }

    private static FeeAccrualDto MapToDto(FeeAccrual accrual)
    {
        return new FeeAccrualDto
        {
            Id = accrual.Id,
            AccrualNumber = accrual.AccrualNumber,
            FeeType = accrual.FeeType.Name,
            FeeTypeDisplayName = accrual.FeeType.DisplayName,
            Status = accrual.Status.Name,
            StatusDisplayName = accrual.Status.DisplayName,
            Period = accrual.Period.Name,
            PeriodDisplayName = accrual.Period.DisplayName,
            MerchantId = accrual.MerchantId,
            CardNumber = accrual.CardNumber,
            TerminalId = accrual.TerminalId,
            GrossAmount = accrual.GrossAmount,
            DiscountAmount = accrual.DiscountAmount,
            NetAmount = accrual.NetAmount,
            PaidAmount = accrual.PaidAmount,
            RemainingAmount = accrual.RemainingAmount,
            AccrualDate = accrual.AccrualDate,
            DueDate = accrual.DueDate,
            PaidDate = accrual.PaidDate,
            AccrualPeriodStart = accrual.AccrualPeriodStart,
            AccrualPeriodEnd = accrual.AccrualPeriodEnd
        };
    }
}