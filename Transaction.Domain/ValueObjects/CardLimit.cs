using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.ValueObjects;

/// <summary>
/// Kart limiti Value Object (LKS için)
/// </summary>
public class CardLimit : ValueObject
{
    public string CardNumber { get; }
    public decimal DailyLimit { get; }
    public decimal MonthlyLimit { get; }
    public decimal DailyUsed { get; }
    public decimal MonthlyUsed { get; }
    public string Currency { get; }

    private CardLimit(string cardNumber, decimal dailyLimit, decimal monthlyLimit,
        decimal dailyUsed, decimal monthlyUsed, string currency)
    {
        CardNumber = cardNumber;
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
        DailyUsed = dailyUsed;
        MonthlyUsed = monthlyUsed;
        Currency = currency;
    }

    public static CardLimit Create(string cardNumber, decimal dailyLimit, decimal monthlyLimit,
        decimal dailyUsed = 0, decimal monthlyUsed = 0, string currency = "TRY")
    {
        return new CardLimit(cardNumber, dailyLimit, monthlyLimit, dailyUsed, monthlyUsed, currency);
    }

    /// <summary>
    /// Kalan günlük limit
    /// </summary>
    public decimal RemainingDailyLimit => Math.Max(0, DailyLimit - DailyUsed);

    /// <summary>
    /// Kalan aylık limit
    /// </summary>
    public decimal RemainingMonthlyLimit => Math.Max(0, MonthlyLimit - MonthlyUsed);

    /// <summary>
    /// İşlem yapılabilir mi?
    /// </summary>
    public bool CanProcess(decimal amount)
    {
        return amount <= RemainingDailyLimit && amount <= RemainingMonthlyLimit;
    }

    /// <summary>
    /// Limit kullanımı sonrası yeni limit oluştur
    /// </summary>
    public CardLimit UseLimit(decimal amount)
    {
        return new CardLimit(
            CardNumber,
            DailyLimit,
            MonthlyLimit,
            DailyUsed + amount,
            MonthlyUsed + amount,
            Currency);
    }

    /// <summary>
    /// Limit iadesi sonrası yeni limit oluştur
    /// </summary>
    public CardLimit ReleaseLimit(decimal amount)
    {
        return new CardLimit(
            CardNumber,
            DailyLimit,
            MonthlyLimit,
            Math.Max(0, DailyUsed - amount),
            Math.Max(0, MonthlyUsed - amount),
            Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CardNumber;
        yield return DailyLimit;
        yield return MonthlyLimit;
        yield return DailyUsed;
        yield return MonthlyUsed;
        yield return Currency;
    }
}