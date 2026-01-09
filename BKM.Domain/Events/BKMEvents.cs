using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Events;

/// <summary>
/// Switch mesajı alındı
/// </summary>
public class SwitchMessageReceivedEvent : DomainEvent
{
    public Guid MessageId { get; }
    public string MessageType { get; }
    public string STAN { get; }
    public string CardNumber { get; }
    public decimal Amount { get; }

    public SwitchMessageReceivedEvent(Guid messageId, string messageType, string stan, string cardNumber, decimal amount)
    {
        MessageId = messageId;
        MessageType = messageType;
        STAN = stan;
        CardNumber = cardNumber;
        Amount = amount;
    }
}

/// <summary>
/// Authorization işlendi
/// </summary>
public class AuthorizationProcessedEvent : DomainEvent
{
    public Guid MessageId { get; }
    public string STAN { get; }
    public string ResponseCode { get; }
    public string AuthCode { get; }
    public bool IsApproved { get; }

    public AuthorizationProcessedEvent(Guid messageId, string stan, string responseCode, string authCode, bool isApproved)
    {
        MessageId = messageId;
        STAN = stan;
        ResponseCode = responseCode;
        AuthCode = authCode;
        IsApproved = isApproved;
    }
}

/// <summary>
/// Clearing kaydı oluşturuldu
/// </summary>
public class ClearingRecordCreatedEvent : DomainEvent
{
    public Guid ClearingId { get; }
    public string STAN { get; }
    public decimal Amount { get; }
    public string AcquirerBank { get; }
    public string IssuerBank { get; }

    public ClearingRecordCreatedEvent(Guid clearingId, string stan, decimal amount, string acquirerBank, string issuerBank)
    {
        ClearingId = clearingId;
        STAN = stan;
        Amount = amount;
        AcquirerBank = acquirerBank;
        IssuerBank = issuerBank;
    }
}

/// <summary>
/// Settlement tamamlandı
/// </summary>
public class SettlementCompletedEvent : DomainEvent
{
    public Guid SettlementId { get; }
    public string SettlementDate { get; }
    public int TransactionCount { get; }
    public decimal TotalAmount { get; }

    public SettlementCompletedEvent(Guid settlementId, string settlementDate, int transactionCount, decimal totalAmount)
    {
        SettlementId = settlementId;
        SettlementDate = settlementDate;
        TransactionCount = transactionCount;
        TotalAmount = totalAmount;
    }
}

/// <summary>
/// Mesaj yönlendirildi
/// </summary>
public class MessageRoutedEvent : DomainEvent
{
    public Guid MessageId { get; }
    public string SourceBank { get; }
    public string DestinationBank { get; }
    public string RoutingKey { get; }

    public MessageRoutedEvent(Guid messageId, string sourceBank, string destinationBank, string routingKey)
    {
        MessageId = messageId;
        SourceBank = sourceBank;
        DestinationBank = destinationBank;
        RoutingKey = routingKey;
    }
}