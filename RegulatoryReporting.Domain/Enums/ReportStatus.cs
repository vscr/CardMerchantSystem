using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Enums;

/// <summary>
/// Rapor durumları
/// </summary>
public class ReportStatus : Enumeration
{
    public static readonly ReportStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly ReportStatus Generating = new(2, "Generating", "Oluşturuluyor");
    public static readonly ReportStatus Generated = new(3, "Generated", "Oluşturuldu");
    public static readonly ReportStatus Validated = new(4, "Validated", "Doğrulandı");
    public static readonly ReportStatus Submitted = new(5, "Submitted", "Gönderildi");
    public static readonly ReportStatus Accepted = new(6, "Accepted", "Kabul Edildi");
    public static readonly ReportStatus Rejected = new(7, "Rejected", "Reddedildi");
    public static readonly ReportStatus Failed = new(8, "Failed", "Başarısız");

    private ReportStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanRegenerate => this == Failed || this == Rejected;
    public bool CanSubmit => this == Generated || this == Validated;
    public bool CanValidate => this == Generated;
    public bool IsFinal => this == Accepted;
}