using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Entities;

/// <summary>
/// Kart limit tanımı.
/// Type = "DEFAULT" → Tüm kartlar için global limit
/// Type = "CARD" → Belirli kart için özel limit
/// Type = "BIN" → BIN bazlı limit (örn: 411111 ile başlayan tüm kartlar)
/// </summary>
public class CardLimitDefinition : Entity
{
    public string LimitType { get; private set; } = null!;     // DEFAULT, CARD, BIN
    public string? TargetValue { get; private set; }            // null (default), kart no, BIN
    public decimal DailyLimit { get; private set; }
    public decimal MonthlyLimit { get; private set; }
    public decimal? SingleTransactionLimit { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public bool IsActive { get; private set; } = true;
    public string? Description { get; private set; }

    private CardLimitDefinition() { }

    public CardLimitDefinition(
        string limitType, string? targetValue,
        decimal dailyLimit, decimal monthlyLimit,
        decimal? singleTransactionLimit,
        string currency, string? description, string createdBy)
    {
        LimitType = limitType;
        TargetValue = targetValue;
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
        SingleTransactionLimit = singleTransactionLimit;
        Currency = currency;
        Description = description;
        CreatedBy = createdBy;
    }

    public void Update(
        decimal dailyLimit, decimal monthlyLimit,
        decimal? singleTransactionLimit, string? description, string updatedBy)
    {
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
        SingleTransactionLimit = singleTransactionLimit;
        Description = description;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive(bool isActive, string updatedBy)
    {
        IsActive = isActive;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Global default limit oluştur
    /// </summary>
    public static CardLimitDefinition CreateDefault(
        decimal dailyLimit, decimal monthlyLimit,
        decimal? singleTransactionLimit, string createdBy)
    {
        return new CardLimitDefinition(
            "DEFAULT", null, dailyLimit, monthlyLimit,
            singleTransactionLimit, "TRY", "Sistem varsayılan limiti", createdBy);
    }
}