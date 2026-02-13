namespace Fraud.Domain.Enums;

/// <summary>
/// Fraud kontrol sonucu durumu.
/// PayGuard'daki FraudStatus karşılığı.
/// </summary>
public enum FraudStatus
{
    /// <summary>Henüz kontrol edilmedi</summary>
    NotChecked = 0,

    /// <summary>Temiz — hiçbir senaryoya takılmadı</summary>
    Clean = 1,

    /// <summary>Şüpheli — en az bir senaryoya takıldı</summary>
    Suspicious = 2,

    /// <summary>Fraud — yüksek skorlu senaryoya takıldı, işlem reddedildi</summary>
    Fraudulent = 3,

    /// <summary>Simülasyon — senaryo tetiklendi ama sadece kayıt amaçlı</summary>
    SimulationHit = 4,

    /// <summary>Manuel inceleme bekliyor</summary>
    PendingReview = 5
}