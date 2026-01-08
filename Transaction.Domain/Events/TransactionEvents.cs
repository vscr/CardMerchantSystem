using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Events;

/// <summary>
/// İşlem oluşturuldu
/// </summary>
public class TransactionCreatedEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public decimal Amount { get; }
    public string CardNumberMasked { get; }

    public TransactionCreatedEvent(Guid transactionId, string referenceNumber, decimal amount, string cardNumberMasked)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        Amount = amount;
        CardNumberMasked = cardNumberMasked;
    }
}

/// <summary>
/// İşlem onaylandı
/// </summary>
public class TransactionApprovedEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public string AuthorizationCode { get; }
    public decimal Amount { get; }

    public TransactionApprovedEvent(Guid transactionId, string referenceNumber, string authorizationCode, decimal amount)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        AuthorizationCode = authorizationCode;
        Amount = amount;
    }
}

/// <summary>
/// İşlem reddedildi
/// </summary>
public class TransactionDeclinedEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public string DeclineReason { get; }

    public TransactionDeclinedEvent(Guid transactionId, string referenceNumber, string declineReason)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        DeclineReason = declineReason;
    }
}

/// <summary>
/// Fraud tespit edildi
/// </summary>
public class FraudDetectedEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public string CardNumberMasked { get; }
    public decimal Amount { get; }
    public string FraudReason { get; }

    public FraudDetectedEvent(Guid transactionId, string referenceNumber, string cardNumberMasked, decimal amount, string fraudReason)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        CardNumberMasked = cardNumberMasked;
        Amount = amount;
        FraudReason = fraudReason;
    }
}

/// <summary>
/// İşlem takas edildi
/// </summary>
public class TransactionSettledEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public DateTime SettledAt { get; }

    public TransactionSettledEvent(Guid transactionId, string referenceNumber, DateTime settledAt)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        SettledAt = settledAt;
    }
}

/// <summary>
/// Limit güncellendi
/// </summary>
public class LimitUpdatedEvent : DomainEvent
{
    public string CardNumber { get; }
    public decimal PreviousDailyUsed { get; }
    public decimal NewDailyUsed { get; }
    public decimal PreviousMonthlyUsed { get; }
    public decimal NewMonthlyUsed { get; }

    public LimitUpdatedEvent(string cardNumber, decimal previousDailyUsed, decimal newDailyUsed,
        decimal previousMonthlyUsed, decimal newMonthlyUsed)
    {
        CardNumber = cardNumber;
        PreviousDailyUsed = previousDailyUsed;
        NewDailyUsed = newDailyUsed;
        PreviousMonthlyUsed = previousMonthlyUsed;
        NewMonthlyUsed = newMonthlyUsed;
    }
}