using Dispute.Application.DTOs;
using MediatR;

namespace Dispute.Application.Queries;

public record GetOverdueDisputesQuery() : IRequest<IReadOnlyList<DisputeDto>>;