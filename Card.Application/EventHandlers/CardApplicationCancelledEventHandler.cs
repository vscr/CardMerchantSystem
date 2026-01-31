using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

public class CardApplicationCancelledEventHandler : INotificationHandler<CardApplicationCancelledEvent>
{
    private readonly ILogger<CardApplicationCancelledEventHandler> _logger;

    public CardApplicationCancelledEventHandler(
        ILogger<CardApplicationCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardApplicationCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🚫 [Card] Kart başvurusu iptal edildi - ApplicationId: {ApplicationId}, Reason: {Reason}",
            notification.ApplicationId,
            notification.Reason
        );

        await Task.CompletedTask;
    }
}