using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Campaign.Application.EventHandlers;

/// <summary>
/// Transaction tamamlandığında kampanya puanları hesaplanır
/// </summary>
public class TransactionCompletedIntegrationEventHandler
    : INotificationHandler<TransactionCompletedIntegrationEvent>
{
    private readonly ILogger<TransactionCompletedIntegrationEventHandler> _logger;

    public TransactionCompletedIntegrationEventHandler(
        ILogger<TransactionCompletedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TransactionCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🎯 [Campaign] Kampanya puanları hesaplıyor - TransactionId: {TransactionId}, MerchantCode: {MerchantCode}, Amount: {Amount}",
            notification.TransactionId,
            notification.MerchantCode,
            notification.Amount
        );

        try
        {
            // TODO: Kampanya puanları hesapla
            // await _campaignService.CalculatePointsAsync(...)

            _logger.LogInformation(
                "✅ [Campaign] Kampanya puanları hesaplandı (simulated) - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [Campaign] Kampanya puanları hesaplanamadı - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }

        await Task.CompletedTask;
    }
}