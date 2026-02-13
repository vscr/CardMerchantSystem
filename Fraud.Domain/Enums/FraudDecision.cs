namespace Fraud.Domain.Enums;

/// <summary>
/// Fraud operatörünün aldığı karar.
/// PayGuard'daki FraudAction.FraudDecision karşılığı.
/// </summary>
public enum FraudDecision
{
    /// <summary>Karar bekleniyor</summary>
    Pending = 0,

    /// <summary>İşlem temiz — yanlış alarm</summary>
    Legitimate = 1,

    /// <summary>Fraud onaylandı</summary>
    ConfirmedFraud = 2,

    /// <summary>Şüpheli — izlemeye alındı</summary>
    Watchlist = 3,

    /// <summary>Kart bloke edildi</summary>
    CardBlocked = 4,

    /// <summary>Müşteriye ulaşıldı — doğrulandı</summary>
    CustomerVerified = 5
}