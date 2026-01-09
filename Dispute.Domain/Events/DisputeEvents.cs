using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Events;

/// <summary>
/// İtiraz oluşturuldu
/// </summary>
public class DisputeCreatedEvent : DomainEvent
{
    public Guid DisputeId { get; }
    public string DisputeNumber { get; }
    public Guid TransactionId { get; }
    public decimal Amount { get; }
    public string Reason { get; }

    public DisputeCreatedEvent(Guid disputeId, string disputeNumber, Guid transactionId, decimal amount, string reason)
    {
        DisputeId = disputeId;
        DisputeNumber = disputeNumber;
        TransactionId = transactionId;
        Amount = amount;
        Reason = reason;
    }
}

/// <summary>
/// İtiraz incelemeye alındı
/// </summary>
public class DisputeUnderReviewEvent : DomainEvent
{
    public Guid DisputeId { get; }
    public string DisputeNumber { get; }
    public string AssignedTo { get; }

    public DisputeUnderReviewEvent(Guid disputeId, string disputeNumber, string assignedTo)
    {
        DisputeId = disputeId;
        DisputeNumber = disputeNumber;
        AssignedTo = assignedTo;
    }
}

/// <summary>
/// Üye işyerinden bilgi istendi
/// </summary>
public class InformationRequestedEvent : DomainEvent
{
    public Guid DisputeId { get; }
    public string DisputeNumber { get; }
    public Guid MerchantId { get; }
    public string RequestedInfo { get; }

    public InformationRequestedEvent(Guid disputeId, string disputeNumber, Guid merchantId, string requestedInfo)
    {
        DisputeId = disputeId;
        DisputeNumber = disputeNumber;
        MerchantId = merchantId;
        RequestedInfo = requestedInfo;
    }
}

/// <summary>
/// İtiraz çözüldü
/// </summary>
public class DisputeResolvedEvent : DomainEvent
{
    public Guid DisputeId { get; }
    public string DisputeNumber { get; }
    public bool InFavorOfCustomer { get; }
    public decimal? RefundAmount { get; }
    public string Resolution { get; }

    public DisputeResolvedEvent(Guid disputeId, string disputeNumber, bool inFavorOfCustomer, decimal? refundAmount, string resolution)
    {
        DisputeId = disputeId;
        DisputeNumber = disputeNumber;
        InFavorOfCustomer = inFavorOfCustomer;
        RefundAmount = refundAmount;
        Resolution = resolution;
    }
}

/// <summary>
/// İtiraz bankaya yönlendirildi
/// </summary>
public class DisputeEscalatedEvent : DomainEvent
{
    public Guid DisputeId { get; }
    public string DisputeNumber { get; }
    public string EscalationReason { get; }

    public DisputeEscalatedEvent(Guid disputeId, string disputeNumber, string escalationReason)
    {
        DisputeId = disputeId;
        DisputeNumber = disputeNumber;
        EscalationReason = escalationReason;
    }
}