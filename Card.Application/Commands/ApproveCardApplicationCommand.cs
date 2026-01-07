using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Başvuru onaylama komutu
/// </summary>
public record ApproveCardApplicationCommand(
    Guid ApplicationId,
    string ApproverUsername
) : IRequest<Result>;