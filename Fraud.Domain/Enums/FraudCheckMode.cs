namespace Fraud.Domain.Enums;

/// <summary>
/// Fraud kontrolün çalışma zamanı.
/// PayGuard'daki Online/Offline senaryo ayrımı.
/// Online: Provizyon anında (gerçek zamanlı, ms kritik)
/// Offline: İşlem sonrası (batch, derin analiz)
/// </summary>
public enum FraudCheckMode
{
    /// <summary>Provizyon anında çalışır (max 100ms)</summary>
    Online = 1,

    /// <summary>İşlem sonrası çalışır (batch analiz)</summary>
    Offline = 2,

    /// <summary>Her iki modda da çalışır</summary>
    Both = 3
}