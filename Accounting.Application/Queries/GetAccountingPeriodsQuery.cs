using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using MediatR;

namespace Accounting.Application.Queries;

public record GetAccountingPeriodsQuery(int? Year = null) : IRequest<IReadOnlyList<AccountingPeriodDto>>;
public class GetAccountingPeriodsQueryHandler
    : IRequestHandler<GetAccountingPeriodsQuery, IReadOnlyList<AccountingPeriodDto>>
{
    private readonly IAccountingPeriodRepository _repository;

    public GetAccountingPeriodsQueryHandler(IAccountingPeriodRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AccountingPeriodDto>> Handle(
        GetAccountingPeriodsQuery request,
        CancellationToken cancellationToken)
    {
        var periods = request.Year.HasValue
            ? await _repository.GetByYearAsync(request.Year.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return periods.Select(p => new AccountingPeriodDto
        {
            Id = p.Id,
            PeriodCode = p.PeriodCode,
            PeriodName = p.PeriodName,
            Year = p.Year,
            Month = p.Month,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Status = p.Status.Name,
            StatusDisplayName = p.Status.DisplayName,
            ClosedAt = p.ClosedAt,
            ClosedBy = p.ClosedBy,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}