using Transaction.Application.DTOs;
using Transaction.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Queries;

public class GetCardLimitQueryHandler
    : IRequestHandler<GetCardLimitQuery, Result<LimitInfoDto>>
{
    private readonly ILimitService _limitService;

    public GetCardLimitQueryHandler(ILimitService limitService)
    {
        _limitService = limitService;
    }

    public async Task<Result<LimitInfoDto>> Handle(
        GetCardLimitQuery request,
        CancellationToken cancellationToken)
    {
        var limitResult = await _limitService.GetCardLimitAsync(request.CardNumberMasked, cancellationToken);

        if (limitResult.IsFailure)
            return Result.Failure<LimitInfoDto>(limitResult.Error!, limitResult.ErrorCode);

        var limit = limitResult.Value!;

        return new LimitInfoDto
        {
            CardNumberMasked = limit.CardNumber,
            DailyLimit = limit.DailyLimit,
            MonthlyLimit = limit.MonthlyLimit,
            DailyUsed = limit.DailyUsed,
            MonthlyUsed = limit.MonthlyUsed,
            RemainingDailyLimit = limit.RemainingDailyLimit,
            RemainingMonthlyLimit = limit.RemainingMonthlyLimit,
            Currency = limit.Currency
        };
    }
}