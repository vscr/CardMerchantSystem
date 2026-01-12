using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Komisyon Dağılımı - İşlem bazlı komisyon kırılımı
/// </summary>
public class CommissionBreakdown : Entity
{
    public Guid TransactionId { get; private set; }
    public string MerchantId { get; private set; } = null!;
    public decimal TransactionAmount { get; private set; }

    // Komisyon Kırılımı
    public decimal TotalCommission { get; private set; }
    public decimal BankShare { get; private set; }
    public decimal InterchangeFee { get; private set; }
    public decimal BKMFee { get; private set; }
    public decimal MerchantDiscount { get; private set; }

    // Oranlar
    public decimal CommissionRate { get; private set; }
    public decimal BankShareRate { get; private set; }
    public decimal InterchangeRate { get; private set; }
    public decimal BKMRate { get; private set; }

    // Sonuç
    public decimal MerchantNetAmount { get; private set; }

    // İşlem detayları
    public string? MCC { get; private set; }
    public int InstallmentCount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public Guid? TariffId { get; private set; }

    // EF Core için
    private CommissionBreakdown() { }

    /// <summary>
    /// Komisyon dağılımı oluşturur
    /// </summary>
    public static CommissionBreakdown Create(
        Guid transactionId,
        string merchantId,
        decimal transactionAmount,
        decimal commissionRate,
        decimal bankShareRate,
        decimal interchangeRate,
        decimal bkmRate,
        string? mcc = null,
        int installmentCount = 1,
        Guid? tariffId = null,
        decimal merchantDiscount = 0)
    {
        var totalCommission = Math.Round(transactionAmount * commissionRate / 100, 2);
        var bankShare = Math.Round(totalCommission * bankShareRate / 100, 2);
        var interchangeFee = Math.Round(totalCommission * interchangeRate / 100, 2);
        var bkmFee = Math.Round(totalCommission * bkmRate / 100, 2);

        // Kalan banka payına eklenir
        var remaining = totalCommission - bankShare - interchangeFee - bkmFee;
        bankShare += remaining;

        var breakdown = new CommissionBreakdown
        {
            TransactionId = transactionId,
            MerchantId = merchantId,
            TransactionAmount = transactionAmount,
            TotalCommission = totalCommission,
            BankShare = bankShare,
            InterchangeFee = interchangeFee,
            BKMFee = bkmFee,
            MerchantDiscount = merchantDiscount,
            CommissionRate = commissionRate,
            BankShareRate = bankShareRate,
            InterchangeRate = interchangeRate,
            BKMRate = bkmRate,
            MerchantNetAmount = transactionAmount - totalCommission + merchantDiscount,
            MCC = mcc,
            InstallmentCount = installmentCount,
            TransactionDate = DateTime.UtcNow,
            TariffId = tariffId
        };

        return breakdown;
    }
}