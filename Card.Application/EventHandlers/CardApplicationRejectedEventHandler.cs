using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

/// <summary>
/// Kart başvurusu reddedildiğinde çalışır
/// </summary>
public class CardApplicationRejectedEventHandler : INotificationHandler<CardApplicationRejectedEvent>
{
    private readonly ILogger<CardApplicationRejectedEventHandler> _logger;

    public CardApplicationRejectedEventHandler(
        ILogger<CardApplicationRejectedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardApplicationRejectedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "❌ [Card Module] Kart başvurusu reddedildi - ApplicationId: {ApplicationId}, Reason: {Reason}",
            notification.ApplicationId,
            notification.Reason
        );

        // TODO: İleride buraya iş mantığı eklenebilir:
        // - Müşteriye red bildirimi gönder
        // - CRM sistemine log kaydet

        await Task.CompletedTask;
    }
}