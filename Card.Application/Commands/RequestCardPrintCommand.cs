using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart basım talebi komutu
/// </summary>
public record RequestCardPrintCommand(
    Guid ApplicationId,
    int PrintVendorId,
    string BatchId,
    string OperatorUsername
) : IRequest<Result>;