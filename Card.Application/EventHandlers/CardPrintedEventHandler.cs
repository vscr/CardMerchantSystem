using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

/// <summary>
/// Kart basıldığında çalışır
/// </summary>
public class CardPrintedEventHandler : INotificationHandler<CardPrintedEvent>
{
    private readonly ILogger<CardPrintedEventHandler> _logger;

    public CardPrintedEventHandler(
        ILogger<CardPrintedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardPrintedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🖨️ [Card Module] Kart basıldı - ApplicationId: {ApplicationId}, MaskedCardNumber: {MaskedCardNumber}",
            notification.ApplicationId,
            notification.MaskedCardNumber  
        );

        await Task.CompletedTask;
    }
}