using Dispute.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Queries;

public record GetDisputeDetailQuery(Guid Id) : IRequest<Result<DisputeDetailDto>>;