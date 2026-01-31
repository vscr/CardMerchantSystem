using Transaction.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transaction.Application.EventHandlers;

public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
{
    private readonly ILogger<TransactionCreatedEventHandler> _logger;

    public TransactionCreatedEventHandler(
        ILogger<TransactionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🆕 [Transaction] İşlem oluşturuldu - TransactionId: {TransactionId}, ReferenceNumber: {ReferenceNumber}, Amount: {Amount}",
            notification.TransactionId,
            notification.ReferenceNumber,
            notification.Amount
        );

        await Task.CompletedTask;
    }
}