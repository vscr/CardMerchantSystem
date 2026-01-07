using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Events;

/// <summary>
/// Kart başvurusu oluşturuldu
/// </summary>
public class CardApplicationCreatedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    public string CardType { get; }

    public CardApplicationCreatedEvent(Guid applicationId, string customerTckn, string cardType)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
        CardType = cardType;
    }
}

/// <summary>
/// Kart başvurusu onaylandı
/// </summary>
public class CardApplicationApprovedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }

    public CardApplicationApprovedEvent(Guid applicationId, string customerTckn)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
    }
}

/// <summary>
/// Kart başvurusu reddedildi
/// </summary>
public class CardApplicationRejectedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    public string Reason { get; }

    public CardApplicationRejectedEvent(Guid applicationId, string customerTckn, string reason)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
        Reason = reason;
    }
}

/// <summary>
/// Kart basımı talep edildi
/// </summary>
public class CardPrintRequestedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string Vendor { get; }
    public string BatchId { get; }

    public CardPrintRequestedEvent(Guid applicationId, string vendor, string batchId)
    {
        ApplicationId = applicationId;
        Vendor = vendor;
        BatchId = batchId;
    }
}

/// <summary>
/// Kart basıldı
/// </summary>
public class CardPrintedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string MaskedCardNumber { get; }

    public CardPrintedEvent(Guid applicationId, string maskedCardNumber)
    {
        ApplicationId = applicationId;
        MaskedCardNumber = maskedCardNumber;
    }
}

/// <summary>
/// Kart teslimat başladı
/// </summary>
public class CardDeliveryStartedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string TrackingNumber { get; }
    public string DeliveryAddress { get; }

    public CardDeliveryStartedEvent(Guid applicationId, string trackingNumber, string deliveryAddress)
    {
        ApplicationId = applicationId;
        TrackingNumber = trackingNumber;
        DeliveryAddress = deliveryAddress;
    }
}

/// <summary>
/// Kart teslim edildi
/// </summary>
public class CardDeliveredEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    public string MaskedCardNumber { get; }

    public CardDeliveredEvent(Guid applicationId, string customerTckn, string maskedCardNumber)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
        MaskedCardNumber = maskedCardNumber;
    }
}

/// <summary>
/// Kart başvurusu iptal edildi
/// </summary>
public class CardApplicationCancelledEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    public string Reason { get; }

    public CardApplicationCancelledEvent(Guid applicationId, string customerTckn, string reason)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
        Reason = reason;
    }
}