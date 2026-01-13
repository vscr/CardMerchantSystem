using Statement.Application.DTOs;
using Statement.Domain.Repositories;
using MediatR;

namespace Statement.Application.Queries;

public class GetStatementsByCardNumberQueryHandler
    : IRequestHandler<GetStatementsByCardNumberQuery, IReadOnlyList<CardStatementSummaryDto>>
{
    private readonly ICardStatementRepository _repository;

    public GetStatementsByCardNumberQueryHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CardStatementSummaryDto>> Handle(
        GetStatementsByCardNumberQuery request,
        CancellationToken cancellationToken)
    {
        var statements = await _repository.GetByCardNumberAsync(request.CardNumber, cancellationToken);

        return statements.Select(s => new CardStatementSummaryDto
        {
            Id = s.Id,
            StatementNumber = s.StatementNumber,
            MaskedCardNumber = s.MaskedCardNumber,
            PeriodEndDate = s.PeriodEndDate,
            DueDate = s.DueDate,
            CurrentBalance = s.CurrentBalance,
            MinimumPayment = s.MinimumPayment,
            RemainingBalance = s.RemainingBalance,
            Status = s.Status.Name,
            StatusDisplayName = s.Status.DisplayName
        }).ToList();
    }
}