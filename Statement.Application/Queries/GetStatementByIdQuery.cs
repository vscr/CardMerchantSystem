using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Queries;

public record GetStatementByIdQuery(Guid Id) : IRequest<Result<CardStatementDto>>;