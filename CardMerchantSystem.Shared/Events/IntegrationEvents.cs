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
/// İşlem onaylandığında fırlatılır
/// Cross-module: Campaign (puan), Fee (komisyon), Accounting dinler
/// </summary>
public class TransactionCompletedIntegrationEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public decimal Amount { get; }
    public string CardNumberMasked { get; }
    public Guid MerchantId { get; }
    public string MerchantCode { get; }
    public Guid TerminalId { get; }
    public string TerminalCode { get; }
    public string AuthorizationCode { get; }

    public TransactionCompletedIntegrationEvent(
        Guid transactionId,
        string referenceNumber,
        decimal amount,
        string cardNumberMasked,
        Guid merchantId,
        string merchantCode,
        Guid terminalId,
        string terminalCode,
        string authorizationCode)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        Amount = amount;
        CardNumberMasked = cardNumberMasked;
        MerchantId = merchantId;
        MerchantCode = merchantCode;
        TerminalId = terminalId;
        TerminalCode = terminalCode;
        AuthorizationCode = authorizationCode;
    }
}

/// <summary>
/// Fraud tespit edildiğinde fırlatılır
/// Cross-module: Dispute, Accounting dinler
/// </summary>
public class FraudDetectedIntegrationEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }
    public string CardNumberMasked { get; }
    public decimal Amount { get; }
    public string FraudReason { get; }

    public FraudDetectedIntegrationEvent(
        Guid transactionId,
        string referenceNumber,
        string cardNumberMasked,
        decimal amount,
        string fraudReason)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
        CardNumberMasked = cardNumberMasked;
        Amount = amount;
        FraudReason = fraudReason;
    }
}