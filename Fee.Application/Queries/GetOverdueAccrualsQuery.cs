using Fee.Application.DTOs;
using MediatR;

namespace Fee.Application.Queries;

public record GetOverdueAccrualsQuery() : IRequest<IReadOnlyList<FeeAccrualDto>>;