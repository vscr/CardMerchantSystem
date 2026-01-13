using Statement.Application.DTOs;
using MediatR;

namespace Statement.Application.Queries;

public record GetStatementsByCardNumberQuery(string CardNumber) : IRequest<IReadOnlyList<CardStatementSummaryDto>>;