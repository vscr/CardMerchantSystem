using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public record CreateStatementPeriodConfigCommand(CreateStatementPeriodConfigDto Dto) : IRequest<Result<StatementPeriodConfigDto>>;