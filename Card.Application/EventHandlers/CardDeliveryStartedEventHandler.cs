using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

/// <summary>
/// Kart teslimatı başladığında çalışır
/// </summary>
public class CardDeliveryStartedEventHandler : INotificationHandler<CardDeliveryStartedEvent>
{
    private readonly ILogger<CardDeliveryStartedEventHandler> _logger;

    public CardDeliveryStartedEventHandler(
        ILogger<CardDeliveryStartedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardDeliveryStartedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🚚 [Card Module] Kart teslimatı başladı - ApplicationId: {ApplicationId}, TrackingNumber: {TrackingNumber}",
            notification.ApplicationId,
            notification.TrackingNumber
        );

        await Task.CompletedTask;
    }
}