using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Entities;

/// <summary>
/// Bloke kuralı
/// </summary>
public class BlockRule : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Kural tipi
    public BlockReason TriggerReason { get; private set; } = null!;
    public AlertSeverity Severity { get; private set; } = null!;

    // Eşik değerleri
    public decimal? AmountThreshold { get; private set; }
    public int? CountThreshold { get; private set; }
    public int? TimeWindowMinutes { get; private set; }

    // Fraud skoru eşiği
    public int? FraudScoreThreshold { get; private set; }

    // Otomatik bloke
    public bool AutoBlockEnabled { get; private set; }
    public int BlockDurationMinutes { get; private set; }

    // Durum
    public bool IsActive { get; private set; }

    // Öncelik
    public int Priority { get; private set; }

    private BlockRule() { }

    public static Result<BlockRule> Create(
        string code,
        string name,
        string description,
        BlockReason triggerReason,
        AlertSeverity severity,
        int priority)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<BlockRule>("Kural kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<BlockRule>("Kural adı boş olamaz");

        var rule = new BlockRule
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            TriggerReason = triggerReason,
            Severity = severity,
            Priority = priority,
            AutoBlockEnabled = severity.RequiresImmediateBlock,
            BlockDurationMinutes = 1440, // Varsayılan 24 saat
            IsActive = true
        };

        return rule;
    }

    /// <summary>
    /// Tutar eşiği ayarla
    /// </summary>
    public void SetAmountThreshold(decimal amount)
    {
        AmountThreshold = amount;
    }

    /// <summary>
    /// Sayı eşiği ayarla
    /// </summary>
    public void SetCountThreshold(int count, int timeWindowMinutes)
    {
        CountThreshold = count;
        TimeWindowMinutes = timeWindowMinutes;
    }

    /// <summary>
    /// Fraud skoru eşiği ayarla
    /// </summary>
    public void SetFraudScoreThreshold(int score)
    {
        FraudScoreThreshold = score;
    }

    /// <summary>
    /// Otomatik bloke ayarla
    /// </summary>
    public void SetAutoBlock(bool enabled, int durationMinutes)
    {
        AutoBlockEnabled = enabled;
        BlockDurationMinutes = durationMinutes;
    }

    /// <summary>
    /// Kuralı aktif et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Kuralı deaktif et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}