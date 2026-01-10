using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Enums;

/// <summary>
/// HSM Komut Tipleri (Thales PayShield komutları)
/// </summary>
public class HSMCommandType : Enumeration
{
    // PIN İşlemleri
    public static readonly HSMCommandType GeneratePINBlock = new(1, "BA", "Generate PIN Block");
    public static readonly HSMCommandType VerifyPIN = new(2, "DA", "Verify Terminal PIN");
    public static readonly HSMCommandType TranslatePINBlock = new(3, "CA", "Translate PIN Block");
    public static readonly HSMCommandType GenerateRandomPIN = new(4, "JA", "Generate Random PIN");
    public static readonly HSMCommandType ChangePIN = new(5, "DC", "Verify and Generate PIN Change");

    // CVV İşlemleri
    public static readonly HSMCommandType GenerateCVV = new(6, "CW", "Generate CVV");
    public static readonly HSMCommandType VerifyCVV = new(7, "CY", "Verify CVV");
    public static readonly HSMCommandType GenerateCVV2 = new(8, "DG", "Generate CVV2");
    public static readonly HSMCommandType VerifyCVV2 = new(9, "DM", "Verify CVV2");

    // Kart İşlemleri
    public static readonly HSMCommandType GenerateARQC = new(10, "KQ", "Generate ARQC");
    public static readonly HSMCommandType VerifyARQC = new(11, "KR", "Verify ARQC and Generate ARPC");
    public static readonly HSMCommandType GenerateMAC = new(12, "MS", "Generate MAC");
    public static readonly HSMCommandType VerifyMAC = new(13, "MQ", "Verify MAC");

    // Key Yönetimi
    public static readonly HSMCommandType GenerateKey = new(14, "A0", "Generate Key");
    public static readonly HSMCommandType ImportKey = new(15, "A6", "Import Key");
    public static readonly HSMCommandType ExportKey = new(16, "A8", "Export Key");
    public static readonly HSMCommandType TranslateKey = new(17, "B2", "Translate Key");

    // Şifreleme
    public static readonly HSMCommandType EncryptData = new(18, "M0", "Encrypt Data Block");
    public static readonly HSMCommandType DecryptData = new(19, "M2", "Decrypt Data Block");

    // Diagnostik
    public static readonly HSMCommandType HealthCheck = new(20, "NC", "Health Check / Diagnostics");

    private HSMCommandType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Response komut kodu
    /// </summary>
    public string ResponseCode => Name.Length == 2
        ? $"{Name[0]}{(char)(Name[1] + 1)}"
        : Name;
}