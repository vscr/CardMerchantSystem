using CardMerchantSystem.Shared.Kernel;

namespace EarlyBlockResolution.Domain.Enums;

/// <summary>
/// Doğrulama sonuçları
/// </summary>
public class VerificationResult : Enumeration
{
    public static readonly VerificationResult Pending = new(1, "Pending", "Beklemede");
    public static readonly VerificationResult Verified = new(2, "Verified", "Doğrulandı");
    public static readonly VerificationResult Failed = new(3, "Failed", "Başarısız");
    public static readonly VerificationResult Expired = new(4, "Expired", "Süresi Doldu");
    public static readonly VerificationResult Cancelled = new(5, "Cancelled", "İptal Edildi");

    private VerificationResult(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool IsSuccess => this == Verified;
    public bool IsFinal => this == Verified || this == Failed || this == Expired || this == Cancelled;
}