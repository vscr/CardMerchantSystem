using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HSM.Application.EventHandlers;

/// <summary>
/// Card modülünden gelen CardApplicationApprovedIntegrationEvent'i dinler
/// Kart için CVV ve PIN üretir
/// </summary>
public class CardApplicationApprovedIntegrationEventHandler
    : INotificationHandler<CardApplicationApprovedIntegrationEvent>
{
    private readonly ILogger<CardApplicationApprovedIntegrationEventHandler> _logger;

    public CardApplicationApprovedIntegrationEventHandler(
        ILogger<CardApplicationApprovedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CardApplicationApprovedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🔐 [HSM] Kart güvenlik verileri üretiliyor - ApplicationId: {ApplicationId}",
            notification.ApplicationId
        );

        try
        {
            // TODO: CVV ve PIN üretimi
            // await _hsmService.GenerateCVVAsync(...)
            // await _hsmService.GenerateEncryptedPINAsync(...)

            _logger.LogInformation(
                "✅ [HSM] Kart güvenlik verileri üretildi (simulated)"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [HSM] Kart güvenlik verileri üretilemedi - ApplicationId: {ApplicationId}",
                notification.ApplicationId
            );
        }

        await Task.CompletedTask;
    }
}