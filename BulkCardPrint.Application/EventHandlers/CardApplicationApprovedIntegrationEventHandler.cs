using CardMerchantSystem.Shared.Events; // ← Shared event
using MediatR;
using Microsoft.Extensions.Logging;

namespace BulkCardPrint.Application.EventHandlers;

/// <summary>
/// Card modülünden gelen CardApplicationApprovedIntegrationEvent'i dinler
/// Otomatik kart basım emri oluşturur
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
            "📄 [BulkCardPrint] Kart basım emri oluşturuluyor - ApplicationId: {ApplicationId}, CustomerName: {CustomerName}, CardType: {CardType}",
            notification.ApplicationId,
            notification.CustomerName,
            notification.CardType
        );

        try
        {
            // TODO: Gerçek kart basım logic'i
            // await _printBatchService.CreateAutoPrintBatchAsync(...)

            _logger.LogInformation(
                "✅ [BulkCardPrint] Kart basım emri oluşturuldu (simulated)"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [BulkCardPrint] Kart basım emri oluşturulamadı - ApplicationId: {ApplicationId}",
                notification.ApplicationId
            );
        }

        await Task.CompletedTask;
    }
}