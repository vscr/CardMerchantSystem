using Merchant.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Merchant.Application.EventHandlers;

public class MerchantSuspendedEventHandler : INotificationHandler<MerchantSuspendedEvent>
{
    private readonly ILogger<MerchantSuspendedEventHandler> _logger;

    public MerchantSuspendedEventHandler(
        ILogger<MerchantSuspendedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(MerchantSuspendedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "⚠️ [Merchant] Üye işyeri askıya alındı - MerchantId: {MerchantId}, MerchantCode: {MerchantCode}, Reason: {Reason}",
            notification.MerchantId,
            notification.MerchantCode,
            notification.Reason
        );

        await Task.CompletedTask;
    }
}