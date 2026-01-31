using Transaction.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transaction.Application.EventHandlers;

public class TransactionDeclinedEventHandler : INotificationHandler<TransactionDeclinedEvent>
{
    private readonly ILogger<TransactionDeclinedEventHandler> _logger;

    public TransactionDeclinedEventHandler(
        ILogger<TransactionDeclinedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TransactionDeclinedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "❌ [Transaction] İşlem reddedildi - TransactionId: {TransactionId}, ReferenceNumber: {ReferenceNumber}, Reason: {Reason}",
            notification.TransactionId,
            notification.ReferenceNumber,
            notification.DeclineReason
        );

        await Task.CompletedTask;
    }
}