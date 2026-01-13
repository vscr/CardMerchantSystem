using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using MediatR;

namespace Accounting.Application.Queries;

public class GetJournalEntriesByPeriodQueryHandler
    : IRequestHandler<GetJournalEntriesByPeriodQuery, IReadOnlyList<JournalEntrySummaryDto>>
{
    private readonly IJournalEntryRepository _repository;

    public GetJournalEntriesByPeriodQueryHandler(IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<JournalEntrySummaryDto>> Handle(
        GetJournalEntriesByPeriodQuery request,
        CancellationToken cancellationToken)
    {
        var entries = await _repository.GetByPeriodAsync(request.PeriodCode, cancellationToken);

        return entries.Select(e => new JournalEntrySummaryDto
        {
            Id = e.Id,
            EntryNumber = e.EntryNumber,
            EntryDate = e.EntryDate,
            TransactionTypeDisplayName = e.TransactionType.DisplayName,
            StatusDisplayName = e.Status.DisplayName,
            Description = e.Description,
            TotalDebit = e.TotalDebit,
            TotalCredit = e.TotalCredit
        }).ToList();
    }
}