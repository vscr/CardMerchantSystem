using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace HSM.Application.EventHandlers;

/// <summary>
/// Merchant modülünden gelen TerminalActivatedIntegrationEvent'i dinler
/// Terminal için encryption key'leri üretir
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
            "🔐 [HSM] Terminal key'leri üretiliyor - TerminalCode: {TerminalCode}",
            notification.TerminalCode
        );

        try
        {
            // TODO: Terminal master key ve working key üret
            // await _hsmService.GenerateMasterKeyAsync(...)
            // await _hsmService.GenerateWorkingKeyAsync(...)

            _logger.LogInformation(
                "✅ [HSM] Terminal key'leri üretildi (simulated)"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [HSM] Terminal key'leri üretilemedi - TerminalCode: {TerminalCode}",
                notification.TerminalCode
            );
        }

        await Task.CompletedTask;
    }
}