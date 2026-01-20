using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Enums;

/// <summary>
/// Rapor tipleri
/// </summary>
public class ReportType : Enumeration
{
    public static readonly ReportType CardStatistics = new(1, "CardStatistics", "Kart İstatistikleri");
    public static readonly ReportType TransactionVolume = new(2, "TransactionVolume", "İşlem Hacimleri");
    public static readonly ReportType FraudReport = new(3, "FraudReport", "Fraud Raporu");
    public static readonly ReportType LimitUsage = new(4, "LimitUsage", "Limit Kullanım Raporu");
    public static readonly ReportType MerchantActivity = new(5, "MerchantActivity", "Üye İşyeri Aktivite Raporu");
    public static readonly ReportType ChargebackReport = new(6, "ChargebackReport", "Chargeback Raporu");
    public static readonly ReportType SettlementReport = new(7, "SettlementReport", "Takas Raporu");
    public static readonly ReportType AMLReport = new(8, "AMLReport", "Kara Para Aklama Raporu");
    public static readonly ReportType CustomerComplaint = new(9, "CustomerComplaint", "Müşteri Şikayet Raporu");
    public static readonly ReportType RiskAssessment = new(10, "RiskAssessment", "Risk Değerlendirme Raporu");

    private ReportType(int id, string name, string displayName)
        : base(id, name, displayName) { }
}