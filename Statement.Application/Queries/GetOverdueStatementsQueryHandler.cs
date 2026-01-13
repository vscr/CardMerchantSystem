using Statement.Application.DTOs;
using Statement.Domain.Repositories;
using MediatR;

namespace Statement.Application.Queries;

public class GetOverdueStatementsQueryHandler
    : IRequestHandler<GetOverdueStatementsQuery, IReadOnlyList<CardStatementSummaryDto>>
{
    private readonly ICardStatementRepository _repository;

    public GetOverdueStatementsQueryHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CardStatementSummaryDto>> Handle(
        GetOverdueStatementsQuery request,
        CancellationToken cancellationToken)
    {
        var statements = await _repository.GetOverdueStatementsAsync(cancellationToken);

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