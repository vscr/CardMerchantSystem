using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Enums;

/// <summary>
/// Rapor Durumları
/// </summary>
public class ReportStatus : Enumeration
{
    public static readonly ReportStatus Pending = new(1, nameof(Pending), "Bekliyor");
    public static readonly ReportStatus Generating = new(2, nameof(Generating), "Oluşturuluyor");
    public static readonly ReportStatus Generated = new(3, nameof(Generated), "Oluşturuldu");
    public static readonly ReportStatus Delivering = new(4, nameof(Delivering), "Gönderiliyor");
    public static readonly ReportStatus Delivered = new(5, nameof(Delivered), "Teslim Edildi");
    public static readonly ReportStatus Failed = new(6, nameof(Failed), "Başarısız");
    public static readonly ReportStatus Cancelled = new(7, nameof(Cancelled), "İptal");

    private ReportStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool IsCompleted => this == Generated || this == Delivered;
    public bool IsFinal => this == Delivered || this == Failed || this == Cancelled;
}