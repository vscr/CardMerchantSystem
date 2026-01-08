using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Enums;

/// <summary>
/// Fraud kontrol sonuçları
/// </summary>
public class FraudCheckResult : Enumeration
{
    public static readonly FraudCheckResult Pass = new(1, nameof(Pass), "Geçti");
    public static readonly FraudCheckResult Review = new(2, nameof(Review), "İnceleme Gerekli");
    public static readonly FraudCheckResult Reject = new(3, nameof(Reject), "Reddedildi");
    public static readonly FraudCheckResult Error = new(4, nameof(Error), "Hata");

    private FraudCheckResult(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool AllowsTransaction => this == Pass || this == Review;
}