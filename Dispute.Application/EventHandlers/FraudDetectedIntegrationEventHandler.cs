using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dispute.Application.EventHandlers;

/// <summary>
/// Fraud tespit edildiğinde otomatik dispute oluşturur
/// </summary>
public class FraudDetectedIntegrationEventHandler
    : INotificationHandler<FraudDetectedIntegrationEvent>
{
    private readonly ILogger<FraudDetectedIntegrationEventHandler> _logger;

    public FraudDetectedIntegrationEventHandler(
        ILogger<FraudDetectedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(FraudDetectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "⚠️ [Dispute] Fraud tespit edildi, dispute oluşturuluyor - TransactionId: {TransactionId}, CardNumberMasked: {CardNumberMasked}",
            notification.TransactionId,
            notification.CardNumberMasked
        );

        try
        {
            // TODO: Otomatik dispute oluştur
            // await _disputeService.CreateFraudDisputeAsync(...)

            _logger.LogInformation(
                "✅ [Dispute] Fraud dispute oluşturuldu (simulated) - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [Dispute] Fraud dispute oluşturulamadı - TransactionId: {TransactionId}",
                notification.TransactionId
            );
        }

        await Task.CompletedTask;
    }
}