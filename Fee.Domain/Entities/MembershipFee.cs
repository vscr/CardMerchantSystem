using Fee.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Aidat Tanımı (Kart/Terminal/POS)
/// </summary>
public class MembershipFee : AggregateRoot
{
    public string FeeName { get; private set; } = null!;
    public FeeType FeeType { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public AccrualPeriod Period { get; private set; } = null!;
    public int GracePeriodDays { get; private set; }
    public decimal LateFeeRate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    // Muafiyet koşulları
    public decimal? MinimumTransactionVolume { get; private set; }
    public int? MinimumTransactionCount { get; private set; }

    // EF Core için
    private MembershipFee() { }

    /// <summary>
    /// Yeni aidat tanımı oluşturur
    /// </summary>
    public static Result<MembershipFee> Create(
        string feeName,
        FeeType feeType,
        decimal amount,
        AccrualPeriod period,
        int gracePeriodDays = 30,
        decimal lateFeeRate = 2.5m,
        string? description = null,
        decimal? minimumTransactionVolume = null,
        int? minimumTransactionCount = null)
    {
        if (string.IsNullOrWhiteSpace(feeName))
            return Result.Failure<MembershipFee>("Aidat adı boş olamaz");

        if (amount < 0)
            return Result.Failure<MembershipFee>("Tutar negatif olamaz");

        if (!feeType.IsPeriodic)
            return Result.Failure<MembershipFee>("Aidat için periyodik ücret tipi seçilmeli");

        var fee = new MembershipFee
        {
            FeeName = feeName,
            FeeType = feeType,
            Amount = amount,
            Period = period,
            GracePeriodDays = gracePeriodDays,
            LateFeeRate = lateFeeRate,
            IsActive = true,
            Description = description,
            MinimumTransactionVolume = minimumTransactionVolume,
            MinimumTransactionCount = minimumTransactionCount
        };

        return fee;
    }

    /// <summary>
    /// Muafiyet kontrolü
    /// </summary>
    public bool IsExempt(decimal transactionVolume, int transactionCount)
    {
        if (MinimumTransactionVolume.HasValue && transactionVolume >= MinimumTransactionVolume.Value)
            return true;

        if (MinimumTransactionCount.HasValue && transactionCount >= MinimumTransactionCount.Value)
            return true;

        return false;
    }

    /// <summary>
    /// Gecikme faizi hesaplar
    /// </summary>
    public decimal CalculateLateFee(decimal unpaidAmount, int daysOverdue)
    {
        if (daysOverdue <= GracePeriodDays)
            return 0;

        var effectiveDays = daysOverdue - GracePeriodDays;
        var dailyRate = LateFeeRate / 365 / 100;
        return Math.Round(unpaidAmount * dailyRate * effectiveDays, 2);
    }

    public void UpdateAmount(decimal newAmount) => Amount = newAmount;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}