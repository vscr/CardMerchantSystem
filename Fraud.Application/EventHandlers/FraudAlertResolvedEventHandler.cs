using Fraud.Domain.Enums;
using Fraud.Domain.Events;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Fraud.Application.EventHandlers;

/// <summary>
/// Alert çözümlendiğinde:
/// 1. Notification gönder (SMS/Email — şimdilik log)
/// 2. Raporlama metrikleri güncelle
/// 
/// PayGuard'daki NotificationService karşılığı.
/// </summary>
public class FraudAlertResolvedEventHandler : INotificationHandler<FraudAlertResolvedEvent>
{
    private readonly ILogger<FraudAlertResolvedEventHandler> _logger;

    public FraudAlertResolvedEventHandler(ILogger<FraudAlertResolvedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(FraudAlertResolvedEvent e, CancellationToken ct)
    {
        _logger.LogInformation(
            "FraudAlert çözümlendi: AlertId={AlertId}, Card={Card}, Decision={Decision}, By={ResolvedBy}",
            e.FraudAlertId, e.MaskedCardNo, e.Decision, e.ResolvedBy);

        // TODO: Notification entegrasyonu
        // - ConfirmedFraud → Müşteriye SMS + Email
        // - CardBlocked → Müşteriye SMS
        // - Legitimate → İç raporlama (false positive tracking)

        if (e.Decision == FraudDecision.ConfirmedFraud)
        {
            _logger.LogWarning(
                "FRAUD ONAYLANDI: Card={Card}, Tx={TxId} — Müşteri bilgilendirilecek",
                e.MaskedCardNo, e.TransactionId);
        }

        return Task.CompletedTask;
    }
}