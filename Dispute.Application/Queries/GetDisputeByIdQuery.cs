using Dispute.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Queries;

public record GetDisputeByIdQuery(Guid Id) : IRequest<Result<DisputeDto>>;