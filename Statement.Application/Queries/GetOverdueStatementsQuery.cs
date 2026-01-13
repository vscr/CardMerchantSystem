using Statement.Application.DTOs;
using MediatR;

namespace Statement.Application.Queries;

public record GetOverdueStatementsQuery() : IRequest<IReadOnlyList<CardStatementSummaryDto>>;