using Merchant.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Merchant.Application.EventHandlers;

/// <summary>
/// Terminal aktif edildiğinde çalışır
/// </summary>
public class TerminalActivatedEventHandler : INotificationHandler<TerminalActivatedEvent>
{
    private readonly ILogger<TerminalActivatedEventHandler> _logger;

    public TerminalActivatedEventHandler(
        ILogger<TerminalActivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TerminalActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🎉 [Merchant Module] Terminal aktif edildi - TerminalId: {TerminalId}, TerminalCode: {TerminalCode}",
            notification.TerminalId,
            notification.TerminalCode
        );

        // TODO: İleride buraya iş mantığı eklenebilir:
        // - BKM modülüne bildirim (cross-module - Seçenek 3'te)
        // - HSM modülüne key üretim talebi (cross-module - Seçenek 3'te)

        await Task.CompletedTask;
    }
}