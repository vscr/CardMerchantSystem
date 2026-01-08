using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Events;

/// <summary>
/// Üye işyeri oluşturuldu
/// </summary>
public class MerchantCreatedEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public string MerchantCode { get; }
    public string TaxNumber { get; }

    public MerchantCreatedEvent(Guid merchantId, string merchantCode, string taxNumber)
    {
        MerchantId = merchantId;
        MerchantCode = merchantCode;
        TaxNumber = taxNumber;
    }
}

/// <summary>
/// Üye işyeri onaylandı
/// </summary>
public class MerchantApprovedEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public string MerchantCode { get; }

    public MerchantApprovedEvent(Guid merchantId, string merchantCode)
    {
        MerchantId = merchantId;
        MerchantCode = merchantCode;
    }
}

/// <summary>
/// Üye işyeri aktif edildi
/// </summary>
public class MerchantActivatedEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public string MerchantCode { get; }

    public MerchantActivatedEvent(Guid merchantId, string merchantCode)
    {
        MerchantId = merchantId;
        MerchantCode = merchantCode;
    }
}

/// <summary>
/// Üye işyeri askıya alındı
/// </summary>
public class MerchantSuspendedEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public string MerchantCode { get; }
    public string Reason { get; }

    public MerchantSuspendedEvent(Guid merchantId, string merchantCode, string reason)
    {
        MerchantId = merchantId;
        MerchantCode = merchantCode;
        Reason = reason;
    }
}

/// <summary>
/// Terminal eklendi
/// </summary>
public class TerminalAddedEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public Guid TerminalId { get; }
    public string TerminalCode { get; }

    public TerminalAddedEvent(Guid merchantId, Guid terminalId, string terminalCode)
    {
        MerchantId = merchantId;
        TerminalId = terminalId;
        TerminalCode = terminalCode;
    }
}

/// <summary>
/// Terminal aktif edildi
/// </summary>
public class TerminalActivatedEvent : DomainEvent
{
    public Guid TerminalId { get; }
    public string TerminalCode { get; }

    public TerminalActivatedEvent(Guid terminalId, string terminalCode)
    {
        TerminalId = terminalId;
        TerminalCode = terminalCode;
    }
}