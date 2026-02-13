using Fraud.Domain.Events;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Fraud.Application.EventHandlers;

/// <summary>
/// Kart fraud sebebiyle bloke edildiğinde:
/// 1. Card modülüne bilgi gönder (integration event)
/// 2. Müşteriye acil SMS gönder
/// 3. Audit log
/// 
/// PayGuard'daki PrivilegeDefinition queue karşılığı.
/// </summary>
public class CardBlockedByFraudEventHandler : INotificationHandler<CardBlockedByFraudEvent>
{
    private readonly ILogger<CardBlockedByFraudEventHandler> _logger;

    public CardBlockedByFraudEventHandler(ILogger<CardBlockedByFraudEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CardBlockedByFraudEvent e, CancellationToken ct)
    {
        _logger.LogCritical(
            "KART BLOKE EDİLDİ (Fraud): Card={Card}, Reason={Reason}, By={BlockedBy}, AlertId={AlertId}",
            e.MaskedCardNo, e.ReasonCode, e.BlockedBy, e.FraudAlertId);

        // TODO: Card modülüne integration event gönder
        // await _mediator.Publish(new CardStatusChangedIntegrationEvent(
        //     e.MaskedCardNo, "Blocked", e.ReasonCode, "FraudModule"));

        // TODO: Acil SMS gönder
        // await _notificationService.SendUrgentSmsAsync(
        //     cardNo, "Kartınız güvenlik sebebiyle bloke edilmiştir. 444-BANK");

        return Task.CompletedTask;
    }
}