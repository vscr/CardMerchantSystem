using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Fraud kuralı. PayGuard'daki Rule entity karşılığı.
/// Her kural bir veya daha fazla koşuldan (FraudRuleCondition) oluşur.
/// 4 farklı tipte çalışabilir: Simple, Complex, Periodic, Linked.
/// 
/// Örnek kurallar:
/// - Simple: "Tutar > 10.000 TL"
/// - Complex: "Tutar > 5.000 TL AND Ülke != TR AND MCC = 5411"
/// - Periodic: "Son 1 saatte aynı karttan 5+ işlem"
/// - Linked: "Kart son 24 saatte 3 farklı ülkede kullanıldı" (SQL bazlı)
/// </summary>
public class FraudRule : Entity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public FraudRuleType RuleType { get; private set; }
    public LogicalOperator LogicalOperator { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Periodic rule: Zaman penceresi (dakika cinsinden).
    /// Örnek: 60 = son 1 saat, 1440 = son 24 saat
    /// </summary>
    public int? PeriodMinutes { get; private set; }

    /// <summary>
    /// Periodic rule: Eşik değeri.
    /// Örnek: Count > 5, Sum > 10000
    /// </summary>
    public decimal? PeriodThreshold { get; private set; }

    /// <summary>
    /// Periodic rule: Toplama fonksiyonu (Count, Sum, Avg, Max, Min)
    /// </summary>
    public string? PeriodFunction { get; private set; }

    /// <summary>
    /// Periodic rule: Gruplama alanı (ShadowCardNo, MerchantId, vb.)
    /// </summary>
    public string? PeriodGroupBy { get; private set; }

    /// <summary>
    /// Linked rule: SQL script.
    /// PayGuard'daki RuleScript karşılığı.
    /// </summary>
    public string? SqlScript { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation
    private readonly List<FraudRuleCondition> _conditions = new();
    public IReadOnlyCollection<FraudRuleCondition> Conditions => _conditions.AsReadOnly();

    private FraudRule() { } // EF Core

    // ── Simple / Complex Rule ──
    public FraudRule(
        string code, string name, string? description,
        FraudRuleType ruleType, LogicalOperator logicalOperator, string createdBy)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        RuleType = ruleType;
        LogicalOperator = logicalOperator;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    // ── Periodic Rule ──
    public static FraudRule CreatePeriodic(
        string code, string name, string? description,
        int periodMinutes, decimal periodThreshold,
        string periodFunction, string periodGroupBy,
        string createdBy)
    {
        return new FraudRule
        {
            Code = code,
            Name = name,
            Description = description,
            RuleType = FraudRuleType.Periodic,
            LogicalOperator = Enums.LogicalOperator.And,
            PeriodMinutes = periodMinutes,
            PeriodThreshold = periodThreshold,
            PeriodFunction = periodFunction,
            PeriodGroupBy = periodGroupBy,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    // ── Linked Rule ──
    public static FraudRule CreateLinked(
        string code, string name, string? description,
        string sqlScript, string createdBy)
    {
        return new FraudRule
        {
            Code = code,
            Name = name,
            Description = description,
            RuleType = FraudRuleType.Linked,
            LogicalOperator = Enums.LogicalOperator.And,
            SqlScript = sqlScript,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddCondition(FraudRuleCondition condition)
    {
        _conditions.Add(condition);
    }

    public void Update(string name, string? description, string updatedBy)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}