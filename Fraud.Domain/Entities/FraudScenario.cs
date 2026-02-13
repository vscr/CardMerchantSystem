using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Fraud senaryosu. PayGuard'daki ScenarioDefinition karşılığı.
/// Bir senaryo = 1 ana kural + opsiyonel filtre kuralı.
/// 
/// Akış: İşlem gelir → Filtre kuralı çalışır (varsa) → Filtre geçerse ana kural çalışır
///       → Kural tetiklenirse → HitScenario kaydı + FraudAlert oluşur
/// 
/// Örnek senaryolar:
/// - "Gece 02-06 arası 5.000 TL üzeri yurtdışı işlem" (Score: 85)
/// - "Aynı karttan 10 dakikada 3+ farklı üye işyerinde işlem" (Score: 90)
/// - "Yeni kart (7 gün) + ilk işlem yurtdışı + 1.000 TL üzeri" (Score: 95)
/// </summary>
public class FraudScenario : Entity
{
    public int ScenarioNo { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    /// <summary>Ana kural — fraud tespiti yapan kural</summary>
    public Guid RuleId { get; private set; }

    /// <summary>Filtre kuralı — bu kural TRUE dönerse senaryo ATLANIR (exclusion)</summary>
    public Guid? FilterRuleId { get; private set; }

    /// <summary>Online/Offline/Both</summary>
    public FraudCheckMode CheckMode { get; private set; }

    /// <summary>
    /// Fraud response kodu. Provizyon yanıtında döner.
    /// Örnek: "05" (Red), "01" (Şüpheli), "00" (Onay)
    /// </summary>
    public string FraudResponseCode { get; private set; } = null!;

    /// <summary>
    /// Risk skoru (0-100). Yüksek skor = yüksek risk.
    /// Birden fazla senaryo tetiklenirse en yüksek skorlu karar alınır.
    /// </summary>
    public int Score { get; private set; }

    /// <summary>Çalışma sırası (düşük önce çalışır)</summary>
    public int RunOrder { get; private set; }

    /// <summary>
    /// Simülasyon modu: TRUE ise senaryo tetiklense bile işlem reddedilmez,
    /// sadece kayıt tutulur. Yeni senaryoları test etmek için kullanılır.
    /// </summary>
    public bool IsSimulation { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation
    public FraudRule Rule { get; private set; } = null!;
    public FraudRule? FilterRule { get; private set; }

    private FraudScenario() { } // EF Core

    public FraudScenario(
        int scenarioNo, string name, string? description,
        Guid ruleId, Guid? filterRuleId,
        FraudCheckMode checkMode, string fraudResponseCode,
        int score, int runOrder, bool isSimulation,
        DateTime? startDate, DateTime? endDate, string createdBy)
    {
        ScenarioNo = scenarioNo;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        RuleId = ruleId;
        FilterRuleId = filterRuleId;
        CheckMode = checkMode;
        FraudResponseCode = fraudResponseCode;
        Score = score is < 0 or > 100
            ? throw new ArgumentOutOfRangeException(nameof(score), "Score 0-100 arası olmalı")
            : score;
        RunOrder = runOrder;
        IsSimulation = isSimulation;
        IsActive = true;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string name, string? description, string fraudResponseCode,
        int score, int runOrder, bool isSimulation, string updatedBy)
    {
        Name = name;
        Description = description;
        FraudResponseCode = fraudResponseCode;
        Score = score;
        RunOrder = runOrder;
        IsSimulation = isSimulation;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate() { IsActive = true; }
    public void Deactivate() { IsActive = false; }
    public void SetSimulation(bool isSimulation) { IsSimulation = isSimulation; }

    /// <summary>Senaryo şu an aktif mi? (tarih aralığı dahil)</summary>
    public bool IsEffective(DateTime now) =>
        IsActive && (StartDate == null || now >= StartDate) && (EndDate == null || now <= EndDate);
}