using CardMerchantSystem.Shared.Kernel;

namespace EarlyBlockResolution.Domain.Enums;

/// <summary>
/// Bloke nedenleri
/// </summary>
public class BlockReason : Enumeration
{
    public static readonly BlockReason SuspiciousTransaction = new(1, "SuspiciousTransaction", "Şüpheli İşlem");
    public static readonly BlockReason LimitExceeded = new(2, "LimitExceeded", "Limit Aşımı");
    public static readonly BlockReason GeographicAnomaly = new(3, "GeographicAnomaly", "Coğrafi Anomali");
    public static readonly BlockReason MultipleFailedPIN = new(4, "MultipleFailedPIN", "Çoklu Hatalı PIN");
    public static readonly BlockReason CardNotPresent = new(5, "CardNotPresent", "Kartsız İşlem Şüphesi");
    public static readonly BlockReason HighRiskMerchant = new(6, "HighRiskMerchant", "Yüksek Riskli İşyeri");
    public static readonly BlockReason VelocityCheck = new(7, "VelocityCheck", "Hız Kontrolü Aşımı");
    public static readonly BlockReason CompromisedCard = new(8, "CompromisedCard", "Ele Geçirilmiş Kart");
    public static readonly BlockReason CustomerRequest = new(9, "CustomerRequest", "Müşteri Talebi");
    public static readonly BlockReason FraudConfirmed = new(10, "FraudConfirmed", "Doğrulanmış Fraud");
    public static readonly BlockReason AMLAlert = new(11, "AMLAlert", "Kara Para Aklama Şüphesi");
    public static readonly BlockReason Other = new(12, "Other", "Diğer");

    private BlockReason(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool RequiresImmediateAction => this == FraudConfirmed || this == CompromisedCard || this == AMLAlert;
    public bool CanAutoResolve => this == LimitExceeded || this == VelocityCheck;
}