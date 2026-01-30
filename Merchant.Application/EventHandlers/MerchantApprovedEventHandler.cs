using Merchant.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Merchant.Application.EventHandlers;

/// <summary>
/// Üye işyeri onaylandığında çalışır
/// </summary>
public class MerchantApprovedEventHandler : INotificationHandler<MerchantApprovedEvent>
{
    private readonly ILogger<MerchantApprovedEventHandler> _logger;

    public MerchantApprovedEventHandler(
        ILogger<MerchantApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(MerchantApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🎉 [Merchant Module] Üye işyeri onaylandı - MerchantId: {MerchantId}, MerchantCode: {MerchantCode}",
            notification.MerchantId,
            notification.MerchantCode
        );

        // TODO: İleride buraya iş mantığı eklenebilir:
        // - Email servisi ile üye işyerine bildirim
        // - Accounting modülüne sözleşme ücreti bildirimi

        await Task.CompletedTask;
    }
}