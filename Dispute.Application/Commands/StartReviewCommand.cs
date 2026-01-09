using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public record StartReviewCommand(Guid DisputeId, string AssignedTo) : IRequest<Result>;