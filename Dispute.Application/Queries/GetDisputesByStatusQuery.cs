using Dispute.Application.DTOs;
using MediatR;

namespace Dispute.Application.Queries;

public record GetDisputesByStatusQuery(int StatusId) : IRequest<IReadOnlyList<DisputeDto>>;