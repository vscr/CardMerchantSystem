using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart basıldı olarak işaretleme komutu
/// </summary>
public record MarkCardAsPrintedCommand(
    Guid ApplicationId,
    string EncryptedCardNumber,
    string MaskedCardNumber,
    string OperatorUsername
) : IRequest<Result>;