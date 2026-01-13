using Accounting.Application.DTOs;
using MediatR;

namespace Accounting.Application.Queries;

public record GetJournalEntriesByPeriodQuery(string PeriodCode) : IRequest<IReadOnlyList<JournalEntrySummaryDto>>;