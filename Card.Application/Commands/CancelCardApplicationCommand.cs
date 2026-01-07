using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Başvuru iptal komutu
/// </summary>
public record CancelCardApplicationCommand(
    Guid ApplicationId,
    string Reason,
    string OperatorUsername
) : IRequest<Result>;