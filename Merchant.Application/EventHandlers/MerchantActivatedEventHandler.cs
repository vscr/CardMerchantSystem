using Merchant.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Merchant.Application.EventHandlers;

/// <summary>
/// Üye işyeri aktif edildiğinde çalışır
/// </summary>
public class MerchantActivatedEventHandler : INotificationHandler<MerchantActivatedEvent>
{
    private readonly ILogger<MerchantActivatedEventHandler> _logger;

    public MerchantActivatedEventHandler(
        ILogger<MerchantActivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(MerchantActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "✅ [Merchant Module] Üye işyeri aktif edildi - MerchantId: {MerchantId}, MerchantCode: {MerchantCode}",
            notification.MerchantId,
            notification.MerchantCode
        );

        await Task.CompletedTask;
    }
}