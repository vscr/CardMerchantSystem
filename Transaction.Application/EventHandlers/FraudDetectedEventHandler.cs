using Transaction.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transaction.Application.EventHandlers;

public class FraudDetectedEventHandler : INotificationHandler<FraudDetectedEvent>
{
    private readonly ILogger<FraudDetectedEventHandler> _logger;

    public FraudDetectedEventHandler(
        ILogger<FraudDetectedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(FraudDetectedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "⚠️ [Transaction] FRAUD TESPİT EDİLDİ - TransactionId: {TransactionId}, ReferenceNumber: {ReferenceNumber}, CardNumberMasked: {CardNumberMasked}, Amount: {Amount}",
            notification.TransactionId,
            notification.ReferenceNumber,
            notification.CardNumberMasked,
            notification.Amount
        );

        await Task.CompletedTask;
    }
}