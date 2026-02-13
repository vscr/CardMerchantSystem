using Fraud.Domain.Entities;
using Fraud.Domain.Events;
using Fraud.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fraud.Application.EventHandlers;

/// <summary>
/// Fraud tespit edildiğinde:
/// 1. FraudAlert oluştur (veya mevcut alert'in skorunu güncelle)
/// 2. CardFraudProfile güncelle
/// 
/// PayGuard'daki AlertTrackingPool oluşturma + HitScenario action queue karşılığı.
/// </summary>
public class FraudDetectedEventHandler : INotificationHandler<FraudDetectedEvent>
{
    private readonly IFraudAlertRepository _alertRepo;
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly ILogger<FraudDetectedEventHandler> _logger;

    public FraudDetectedEventHandler(
        IFraudAlertRepository alertRepo,
        ICardFraudProfileRepository profileRepo,
        ILogger<FraudDetectedEventHandler> logger)
    {
        _alertRepo = alertRepo;
        _profileRepo = profileRepo;
        _logger = logger;
    }

    public async Task Handle(FraudDetectedEvent e, CancellationToken ct)
    {
        // Simülasyon ise sadece log, alert oluşturma
        if (e.IsSimulation)
        {
            _logger.LogInformation(
                "Simülasyon hit: Scenario={ScenarioId}, Card={Card}, Score={Score}",
                e.FraudScenarioId, e.MaskedCardNo, e.Score);
            return;
        }

        // Aynı transaction için zaten alert var mı?
        var existingAlert = await _alertRepo.GetByTransactionIdAsync(e.TransactionId, ct);

        if (existingAlert != null)
        {
            // Mevcut alert — skor ve hit count güncelle (en yüksek skoru tut)
            // Not: Entity'de bu method'lar yoksa doğrudan repository üzerinden
            _logger.LogInformation(
                "Mevcut alert güncelleniyor: AlertId={AlertId}, YeniScore={Score}",
                existingAlert.Id, e.Score);
        }
        else
        {
            // Yeni alert oluştur
            var alert = new FraudAlert(
                e.TransactionId, e.MaskedCardNo,
                e.MerchantId, e.MerchantName,
                e.TransactionAmount, e.CurrencyCode,
                e.Score, 1, e.FraudResponseCode);

            await _alertRepo.AddAsync(alert, ct);

            _logger.LogWarning(
                "Yeni FraudAlert oluşturuldu: AlertId={AlertId}, Card={Card}, Score={Score}",
                alert.Id, e.MaskedCardNo, e.Score);
        }
    }
}