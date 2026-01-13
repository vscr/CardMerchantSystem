using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using MediatR;

namespace Accounting.Application.Queries;

public class GetChartOfAccountsQueryHandler
    : IRequestHandler<GetChartOfAccountsQuery, IReadOnlyList<ChartOfAccountDto>>
{
    private readonly IChartOfAccountRepository _repository;

    public GetChartOfAccountsQueryHandler(IChartOfAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ChartOfAccountDto>> Handle(
        GetChartOfAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetAllActiveAsync(cancellationToken);

        return accounts.Select(a => new ChartOfAccountDto
        {
            Id = a.Id,
            AccountCode = a.AccountCode,
            AccountName = a.AccountName,
            Description = a.Description,
            AccountType = a.AccountType.Name,
            AccountTypeDisplayName = a.AccountType.DisplayName,
            ParentAccountId = a.ParentAccountId,
            Level = a.Level,
            IsActive = a.IsActive,
            IsPostable = a.IsPostable,
            CurrencyCode = a.CurrencyCode,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}