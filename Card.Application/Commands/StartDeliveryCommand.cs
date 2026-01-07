using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Teslimat başlatma komutu
/// </summary>
public record StartDeliveryCommand(
    Guid ApplicationId,
    string TrackingNumber,
    string OperatorUsername
) : IRequest<Result>;