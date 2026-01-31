using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

public class CardDeliveredEventHandler : INotificationHandler<CardDeliveredEvent>
{
    private readonly ILogger<CardDeliveredEventHandler> _logger;

    public CardDeliveredEventHandler(
        ILogger<CardDeliveredEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardDeliveredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "📦 [Card] Kart teslim edildi - ApplicationId: {ApplicationId}, MaskedCardNumber: {MaskedCardNumber}",
            notification.ApplicationId,
            notification.MaskedCardNumber
        );

        await Task.CompletedTask;
    }
}