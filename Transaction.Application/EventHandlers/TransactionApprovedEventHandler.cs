using Transaction.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transaction.Application.EventHandlers;

public class TransactionApprovedEventHandler : INotificationHandler<TransactionApprovedEvent>
{
    private readonly ILogger<TransactionApprovedEventHandler> _logger;

    public TransactionApprovedEventHandler(
        ILogger<TransactionApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TransactionApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "✅ [Transaction] İşlem onaylandı - TransactionId: {TransactionId}, ReferenceNumber: {ReferenceNumber}, AuthorizationCode: {AuthorizationCode}",
            notification.TransactionId,
            notification.ReferenceNumber,
            notification.AuthorizationCode
        );

        await Task.CompletedTask;
    }
}