using Transaction.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transaction.Application.EventHandlers;

public class TransactionSettledEventHandler : INotificationHandler<TransactionSettledEvent>
{
    private readonly ILogger<TransactionSettledEventHandler> _logger;

    public TransactionSettledEventHandler(
        ILogger<TransactionSettledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TransactionSettledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🏦 [Transaction] İşlem takas edildi - TransactionId: {TransactionId}, ReferenceNumber: {ReferenceNumber}, SettledAt: {SettledAt}",
            notification.TransactionId,
            notification.ReferenceNumber,
            notification.SettledAt
        );

        await Task.CompletedTask;
    }
}