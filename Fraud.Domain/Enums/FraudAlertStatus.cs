namespace Fraud.Domain.Enums;

/// <summary>
/// Alert havuzu durumu.
/// PayGuard'daki AlertTrackingPool.IsProcessed karşılığı.
/// </summary>
public enum FraudAlertStatus
{
    /// <summary>Yeni oluştu — henüz atanmadı</summary>
    New = 0,

    /// <summary>Operatöre atandı</summary>
    Assigned = 1,

    /// <summary>İnceleniyor</summary>
    InProgress = 2,

    /// <summary>Karar verildi — kapatıldı</summary>
    Resolved = 3,

    /// <summary>Eskalasyon — üst seviyeye iletildi</summary>
    Escalated = 4
}