using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fee.Application.EventHandlers;

/// <summary>
/// Transaction tamamlandığında komisyon hesaplanır
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
            "💰 [Fee] Komisyon hesaplıyor - TransactionId: {TransactionId}, MerchantCode: {MerchantCode}, Amount: {Amount}",
            notification.TransactionId,
            notification.MerchantCode,
            notification.Amount
        );

        try
        {
            // TODO: Komisyon hesapla
            // await _feeService.CalculateCommissionAsync(...)

            _logger.LogInformation(
                "✅ [Fee] Komisyon hesaplandı (simulated) - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [Fee] Komisyon hesaplanamadı - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }

        await Task.CompletedTask;
    }
}