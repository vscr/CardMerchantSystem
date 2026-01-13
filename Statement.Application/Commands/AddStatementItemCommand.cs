using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public record AddStatementItemCommand(AddStatementItemDto Dto) : IRequest<Result<CardStatementDto>>;