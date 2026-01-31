using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Accounting.Application.EventHandlers;

/// <summary>
/// Transaction tamamlandığında muhasebe kayıt oluşturur
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
            "📊 [Accounting] Muhasebe kayıt oluşturuluyor - TransactionId: {TransactionId}, Amount: {Amount}",
            notification.TransactionId,
            notification.Amount
        );

        try
        {
            // TODO: Muhasebe kayıt oluştur
            // await _accountingService.CreateEntryAsync(...)

            _logger.LogInformation(
                "✅ [Accounting] Muhasebe kayıt oluşturuldu (simulated) - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [Accounting] Muhasebe kayıt oluşturulamadı - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }

        await Task.CompletedTask;
    }
}