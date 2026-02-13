namespace Fraud.Domain.Enums;

/// <summary>
/// Kural tipi. PayGuard'daki RuleType karşılığı.
/// Her tip farklı executor ile çalışır (Strategy pattern).
/// </summary>
public enum FraudRuleType
{
    /// <summary>Tek işlem parametrelerini kontrol eder (tutar > X, MCC = Y)</summary>
    Simple = 1,

    /// <summary>Birden fazla koşulun AND/OR kombinasyonu</summary>
    Complex = 2,

    /// <summary>Zaman aralığında toplam/sayım bazlı (son 1 saatte 5+ işlem)</summary>
    Periodic = 3,

    /// <summary>Başka bir kurala bağlı çalışır (SQL script tabanlı)</summary>
    Linked = 4
}