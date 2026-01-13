using Accounting.Application.DTOs;
using MediatR;

namespace Accounting.Application.Queries;

public record GetAccountingPeriodsQuery(int? Year = null) : IRequest<IReadOnlyList<AccountingPeriodDto>>;