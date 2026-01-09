using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public record EscalateDisputeCommand(
    Guid DisputeId,
    string EscalationReason,
    string OperatorUsername
) : IRequest<Result>;