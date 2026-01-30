using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BKM.Application.EventHandlers;

/// <summary>
/// Merchant modülünden gelen TerminalActivatedIntegrationEvent'i dinler
/// Terminal'i BKM Switch'e kaydeder
/// </summary>
public class TerminalActivatedIntegrationEventHandler
    : INotificationHandler<TerminalActivatedIntegrationEvent>
{
    private readonly ILogger<TerminalActivatedIntegrationEventHandler> _logger;

    public TerminalActivatedIntegrationEventHandler(
        ILogger<TerminalActivatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TerminalActivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🔄 [BKM] Terminal BKM Switch'e kaydediliyor - TerminalCode: {TerminalCode}, MerchantCode: {MerchantCode}",
            notification.TerminalCode,
            notification.MerchantCode
        );

        try
        {
            // TODO: BKM API'ye terminal kaydı
            // await _bkmClient.RegisterTerminalAsync(...)

            _logger.LogInformation(
                "✅ [BKM] Terminal BKM Switch'e kaydedildi (simulated)"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [BKM] Terminal BKM Switch'e kaydedilemedi - TerminalCode: {TerminalCode}",
                notification.TerminalCode
            );
        }

        await Task.CompletedTask;
    }
}