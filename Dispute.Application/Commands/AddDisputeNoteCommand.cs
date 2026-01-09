using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public record AddDisputeNoteCommand(
    Guid DisputeId,
    string Note,
    string Username,
    bool IsInternal
) : IRequest<Result>;