using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Enums;

/// <summary>
/// ISO 8583 Response Kodları
/// </summary>
public class ResponseCode : Enumeration
{
    public static readonly ResponseCode Approved = new(1, "00", "Onaylandı");
    public static readonly ResponseCode ReferToIssuer = new(2, "01", "Kartı veren bankayı arayın");
    public static readonly ResponseCode InvalidMerchant = new(3, "03", "Geçersiz üye işyeri");
    public static readonly ResponseCode DoNotHonor = new(4, "05", "Red - İşleme izin verilmedi");
    public static readonly ResponseCode InvalidTransaction = new(5, "12", "Geçersiz işlem");
    public static readonly ResponseCode InvalidAmount = new(6, "13", "Geçersiz tutar");
    public static readonly ResponseCode InvalidCardNumber = new(7, "14", "Geçersiz kart numarası");
    public static readonly ResponseCode NoSuchIssuer = new(8, "15", "Kartı veren banka bulunamadı");
    public static readonly ResponseCode FormatError = new(9, "30", "Format hatası");
    public static readonly ResponseCode LostCard = new(10, "41", "Kayıp kart");
    public static readonly ResponseCode StolenCard = new(11, "43", "Çalıntı kart");
    public static readonly ResponseCode InsufficientFunds = new(12, "51", "Yetersiz bakiye");
    public static readonly ResponseCode ExpiredCard = new(13, "54", "Vadesi geçmiş kart");
    public static readonly ResponseCode IncorrectPIN = new(14, "55", "Hatalı PIN");
    public static readonly ResponseCode ExceedsLimit = new(15, "61", "Limit aşıldı");
    public static readonly ResponseCode RestrictedCard = new(16, "62", "Kısıtlı kart");
    public static readonly ResponseCode SecurityViolation = new(17, "63", "Güvenlik ihlali");
    public static readonly ResponseCode ExceedsFrequency = new(18, "65", "İşlem sıklığı aşıldı");
    public static readonly ResponseCode SystemMalfunction = new(19, "96", "Sistem hatası");
    public static readonly ResponseCode Timeout = new(20, "91", "Zaman aşımı");

    private ResponseCode(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Başarılı mı?
    /// </summary>
    public bool IsApproved => this == Approved;

    /// <summary>
    /// Tekrar denenebilir mi?
    /// </summary>
    public bool IsRetryable => this == SystemMalfunction || this == Timeout;
}