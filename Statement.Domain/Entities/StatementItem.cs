using Statement.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Entities;

/// <summary>
/// Ekstre Kalemi
/// </summary>
public class StatementItem : Entity
{
    public Guid StatementId { get; private set; }
    public StatementItemType ItemType { get; private set; } = null!;
    public DateTime TransactionDate { get; private set; }
    public DateTime PostDate { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? MerchantName { get; private set; }
    public string? MerchantCategory { get; private set; }
    public int? InstallmentNumber { get; private set; }
    public int? TotalInstallments { get; private set; }
    public string? OriginalCurrency { get; private set; }
    public decimal? OriginalAmount { get; private set; }
    public decimal? ExchangeRate { get; private set; }

    // EF Core için
    private StatementItem() { }

    /// <summary>
    /// Yeni ekstre kalemi oluşturur
    /// </summary>
    public static Result<StatementItem> Create(
        Guid statementId,
        StatementItemType itemType,
        DateTime transactionDate,
        string description,
        decimal amount,
        string? referenceNumber = null,
        string? merchantName = null,
        int? installmentNumber = null,
        int? totalInstallments = null,
        string? originalCurrency = null,
        decimal? originalAmount = null,
        decimal? exchangeRate = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<StatementItem>("Açıklama boş olamaz");

        // Alacak kalemleri için tutarı negatif yap
        var effectiveAmount = itemType.IsCredit ? -Math.Abs(amount) : Math.Abs(amount);

        var item = new StatementItem
        {
            StatementId = statementId,
            ItemType = itemType,
            TransactionDate = transactionDate,
            PostDate = DateTime.UtcNow,
            Description = description,
            Amount = effectiveAmount,
            ReferenceNumber = referenceNumber,
            MerchantName = merchantName,
            InstallmentNumber = installmentNumber,
            TotalInstallments = totalInstallments,
            OriginalCurrency = originalCurrency,
            OriginalAmount = originalAmount,
            ExchangeRate = exchangeRate
        };

        return item;
    }

    /// <summary>
    /// Taksit bilgisi formatı
    /// </summary>
    public string? InstallmentInfo =>
        InstallmentNumber.HasValue && TotalInstallments.HasValue
            ? $"{InstallmentNumber}/{TotalInstallments}"
            : null;
}