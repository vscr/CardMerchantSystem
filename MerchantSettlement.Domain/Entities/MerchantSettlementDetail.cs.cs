using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Takas batch detayı - Tek bir işlem kaydı
/// </summary>
public class MerchantSettlementDetail : Entity
{
    public Guid SettlementBatchId { get; private set; }

    // İşlem bilgileri
    public string TransactionId { get; private set; } = null!;
    public string TransactionNumber { get; private set; } = null!;
    public string TransactionType { get; private set; } = null!; // Sale, Refund, Chargeback
    public DateTime TransactionDate { get; private set; }

    // Kart bilgileri
    public string CardNumberMasked { get; private set; } = null!;
    public string CardBrand { get; private set; } = null!; // Visa, Mastercard, Troy

    // Terminal bilgileri
    public string TerminalId { get; private set; } = null!;

    // Tutar bilgileri
    public decimal Amount { get; private set; }
    public decimal CommissionRate { get; private set; }
    public decimal CommissionAmount { get; private set; }
    public decimal FeeAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; } = "TRY";

    // Taksit
    public int InstallmentCount { get; private set; }

    // Orijinal işlem (iade/chargeback için)
    public string? OriginalTransactionId { get; private set; }

    // Referans
    public string? AuthorizationCode { get; private set; }
    public string? ReferenceNumber { get; private set; }

    private MerchantSettlementDetail() { }

    public static MerchantSettlementDetail Create(
        Guid settlementBatchId,
        string transactionId,
        string transactionNumber,
        string transactionType,
        DateTime transactionDate,
        string cardNumberMasked,
        string cardBrand,
        string terminalId,
        decimal amount,
        decimal commissionRate,
        decimal commissionAmount,
        decimal feeAmount,
        int installmentCount = 1,
        string? originalTransactionId = null,
        string? authorizationCode = null,
        string? referenceNumber = null)
    {
        var netAmount = transactionType == "Sale"
            ? amount - commissionAmount - feeAmount
            : -(amount - commissionAmount - feeAmount);

        return new MerchantSettlementDetail
        {
            SettlementBatchId = settlementBatchId,
            TransactionId = transactionId,
            TransactionNumber = transactionNumber,
            TransactionType = transactionType,
            TransactionDate = transactionDate,
            CardNumberMasked = cardNumberMasked,
            CardBrand = cardBrand,
            TerminalId = terminalId,
            Amount = amount,
            CommissionRate = commissionRate,
            CommissionAmount = commissionAmount,
            FeeAmount = feeAmount,
            NetAmount = netAmount,
            InstallmentCount = installmentCount,
            OriginalTransactionId = originalTransactionId,
            AuthorizationCode = authorizationCode,
            ReferenceNumber = referenceNumber
        };
    }
}