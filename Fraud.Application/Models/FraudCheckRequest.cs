namespace Fraud.Application.Models;

/// <summary>
/// Fraud kontrolüne giren işlem verisi.
/// PayGuard'daki TransactionInputParameters karşılığı.
/// Transaction modülünden gelen integration event ile doldurulur.
/// </summary>
public class FraudCheckRequest
{
    // ── İşlem Bilgileri ──
    public Guid TransactionId { get; set; }
    public long? TransactionMessageId { get; set; }
    public DateTime TransactionDate { get; set; }
    public int? TransactionHour { get; set; }

    // ── Tutar ──
    public decimal OriginalAmount { get; set; }
    public string? OriginalCurrencyCode { get; set; }
    public decimal? BillingAmount { get; set; }
    public string? BillingCurrencyCode { get; set; }

    // ── Kart Bilgileri ──
    public string MaskedCardNo { get; set; } = null!;
    public string? CardType { get; set; }
    public string? Bin { get; set; }
    public string? CardCountryCode { get; set; }
    public DateTime? CardActivationDate { get; set; }
    public DateTime? CardExpireDate { get; set; }
    public bool? IsVirtualCard { get; set; }
    public DateTime? FirstTransactionDate { get; set; }

    // ── Üye İşyeri ──
    public string? MerchantId { get; set; }
    public string? MerchantName { get; set; }
    public string? MerchantCity { get; set; }
    public string? MerchantCountryCode { get; set; }
    public string? Mcc { get; set; }
    public string? TerminalId { get; set; }
    public string? TerminalType { get; set; }

    // ── POS / Kanal ──
    public string? PosEntryMode { get; set; }
    public bool? IsEmvTransaction { get; set; }
    public bool? IsPinEntered { get; set; }
    public bool? HasCvv { get; set; }
    public bool? HasCvv2 { get; set; }
    public string? ChannelType { get; set; }
    public string? CavvResult { get; set; }

    // ── İşlem Tipi ──
    public string? Mti { get; set; }
    public string? TranCode { get; set; }
    public string? ProcessingCode { get; set; }
    public bool? IsReversal { get; set; }
    public bool? IsVoid { get; set; }
    public bool? IsClearing { get; set; }

    // ── Müşteri ──
    public string? CustomerNo { get; set; }
    public string? IdentityNumber { get; set; }

    // ── Acquirer ──
    public string? AcquirerId { get; set; }
    public string? AcquirerName { get; set; }
    public string? Rrn { get; set; }
}