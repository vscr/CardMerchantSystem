using Card.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Card.Application.EventHandlers;

/// <summary>
/// Kart başvurusu onaylandığında çalışır
/// </summary>
public class CardApplicationApprovedEventHandler : INotificationHandler<CardApplicationApprovedEvent>
{
    private readonly ILogger<CardApplicationApprovedEventHandler> _logger;

    public CardApplicationApprovedEventHandler(
        ILogger<CardApplicationApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardApplicationApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🎉 [Card Module] Kart başvurusu onaylandı - ApplicationId: {ApplicationId}, CustomerTckn: {CustomerTckn}",
            notification.ApplicationId,
            notification.CustomerTckn
        );

        // TODO: İleride buraya iş mantığı eklenebilir:
        // - Email/SMS gönderimi
        // - Notification servisi
        // - Audit log kaydı

        await Task.CompletedTask;
    }
}