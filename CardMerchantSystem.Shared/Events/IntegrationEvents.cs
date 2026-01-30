using CardMerchantSystem.Shared.Kernel;

namespace CardMerchantSystem.Shared.Events;

// ═══════════════════════════════════════════════════════════════
// CARD MODULE INTEGRATION EVENTS
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Kart başvurusu onaylandığında fırlatılır
/// Cross-module: BulkCardPrint, HSM, Courier dinler
/// </summary>
public class CardApplicationApprovedIntegrationEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    public string CustomerName { get; }
    public string CardType { get; }

    public CardApplicationApprovedIntegrationEvent(
        Guid applicationId,
        string customerTckn,
        string customerName,
        string cardType)
    {
        ApplicationId = applicationId;
        CustomerTckn = customerTckn;
        CustomerName = customerName;
        CardType = cardType;
    }
}

/// <summary>
/// Kart basıldığında fırlatılır
/// Cross-module: HSM (CVV/PIN), Courier dinler
/// </summary>
public class CardPrintedIntegrationEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string MaskedCardNumber { get; }

    public CardPrintedIntegrationEvent(
        Guid applicationId,
        string maskedCardNumber)
    {
        ApplicationId = applicationId;
        MaskedCardNumber = maskedCardNumber;
    }
}

// ═══════════════════════════════════════════════════════════════
// MERCHANT MODULE INTEGRATION EVENTS
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Üye işyeri onaylandığında fırlatılır
/// Cross-module: Email servisi, Accounting dinler
/// </summary>
public class MerchantApprovedIntegrationEvent : DomainEvent
{
    public Guid MerchantId { get; }
    public string MerchantCode { get; }
    public string MerchantName { get; }

    public MerchantApprovedIntegrationEvent(
        Guid merchantId,
        string merchantCode,
        string merchantName)
    {
        MerchantId = merchantId;
        MerchantCode = merchantCode;
        MerchantName = merchantName;
    }
}

/// <summary>
/// Terminal aktif edildiğinde fırlatılır
/// Cross-module: BKM, HSM dinler
/// </summary>
public class TerminalActivatedIntegrationEvent : DomainEvent
{
    public Guid TerminalId { get; }
    public string TerminalCode { get; }
    public Guid MerchantId { get; }
    public string MerchantCode { get; }

    public TerminalActivatedIntegrationEvent(
        Guid terminalId,
        string terminalCode,
        Guid merchantId,
        string merchantCode)
    {
        TerminalId = terminalId;
        TerminalCode = terminalCode;
        MerchantId = merchantId;
        MerchantCode = merchantCode;
    }
}

// ═══════════════════════════════════════════════════════════════
// TRANSACTION MODULE INTEGRATION EVENTS
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Transaction tamamlandığında fırlatılır
/// Cross-module: Campaign (puan), Fee (komisyon), Accounting dinler
/// </summary>
public class TransactionCompletedIntegrationEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public Guid CardId { get; }
    public string CardNumber { get; }
    public Guid MerchantId { get; }
    public string MerchantCode { get; }
    public decimal Amount { get; }
    public string Currency { get; }

    public TransactionCompletedIntegrationEvent(
        Guid transactionId,
        string referenceNumber,
        Guid cardId,
        string cardNumber,
        Guid merchantId,
        string merchantCode,
        decimal amount,
        string currency)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        CardId = cardId;
        CardNumber = cardNumber;
        MerchantId = merchantId;
        MerchantCode = merchantCode;
        Amount = amount;
        Currency = currency;
    }
}