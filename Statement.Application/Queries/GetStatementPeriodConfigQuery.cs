using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Queries;

public record GetStatementPeriodConfigQuery(string CardNumber) : IRequest<Result<StatementPeriodConfigDto>>;