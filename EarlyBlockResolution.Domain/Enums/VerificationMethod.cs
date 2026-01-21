using CardMerchantSystem.Shared.Kernel;

namespace EarlyBlockResolution.Domain.Enums;

/// <summary>
/// Doğrulama yöntemleri
/// </summary>
public class VerificationMethod : Enumeration
{
    public static readonly VerificationMethod SmsOtp = new(1, "SmsOtp", "SMS OTP");
    public static readonly VerificationMethod EmailOtp = new(2, "EmailOtp", "E-posta OTP");
    public static readonly VerificationMethod CallCenter = new(3, "CallCenter", "Çağrı Merkezi");
    public static readonly VerificationMethod MobileApp = new(4, "MobileApp", "Mobil Uygulama");
    public static readonly VerificationMethod Branch = new(5, "Branch", "Şube");
    public static readonly VerificationMethod BiometricVerification = new(6, "BiometricVerification", "Biyometrik Doğrulama");

    private VerificationMethod(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool IsRemote => this == SmsOtp || this == EmailOtp || this == MobileApp;
    public bool RequiresAgent => this == CallCenter || this == Branch;
}