using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public record CreateStatementCommand(CreateStatementDto Dto) : IRequest<Result<CardStatementDto>>;