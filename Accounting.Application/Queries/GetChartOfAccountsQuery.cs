using Accounting.Application.DTOs;
using MediatR;

namespace Accounting.Application.Queries;

public record GetChartOfAccountsQuery() : IRequest<IReadOnlyList<ChartOfAccountDto>>;