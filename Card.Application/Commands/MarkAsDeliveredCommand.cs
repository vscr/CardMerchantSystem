using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart teslim edildi komutu
/// </summary>
public record MarkAsDeliveredCommand(
    Guid ApplicationId,
    string OperatorUsername
) : IRequest<Result>;