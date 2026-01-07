using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Başvuru reddetme komutu
/// </summary>
public record RejectCardApplicationCommand(
    Guid ApplicationId,
    string Reason,
    string RejectorUsername
) : IRequest<Result>;