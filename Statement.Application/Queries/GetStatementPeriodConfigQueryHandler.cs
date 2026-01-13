using Statement.Application.DTOs;
using Statement.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Queries;

public class GetStatementPeriodConfigQueryHandler
    : IRequestHandler<GetStatementPeriodConfigQuery, Result<StatementPeriodConfigDto>>
{
    private readonly IStatementPeriodConfigRepository _repository;

    public GetStatementPeriodConfigQueryHandler(IStatementPeriodConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<StatementPeriodConfigDto>> Handle(
        GetStatementPeriodConfigQuery request,
        CancellationToken cancellationToken)
    {
        var config = await _repository.GetByCardNumberAsync(request.CardNumber, cancellationToken);

        if (config == null)
            return Result.Failure<StatementPeriodConfigDto>("Kesim ayarı bulunamadı", ErrorCodes.NotFound);

        return new StatementPeriodConfigDto
        {
            Id = config.Id,
            CardNumber = config.CardNumber,
            StatementDay = config.StatementDay,
            PaymentDueDays = config.PaymentDueDays,
            InterestRate = config.InterestRate,
            CashAdvanceInterestRate = config.CashAdvanceInterestRate,
            MinimumPaymentRate = config.MinimumPaymentRate,
            MinimumPaymentAmount = config.MinimumPaymentAmount,
            IsActive = config.IsActive,
            CreatedAt = config.CreatedAt
        };
    }
}