using CardMerchantSystem.Shared.Kernel;

namespace BulkCardPrint.Domain.Enums;

/// <summary>
/// Basım item durumları
/// </summary>
public class PrintItemStatus : Enumeration
{
    public static readonly PrintItemStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly PrintItemStatus InProduction = new(2, "InProduction", "Üretimde");
    public static readonly PrintItemStatus Printed = new(3, "Printed", "Basıldı");
    public static readonly PrintItemStatus QualityFailed = new(4, "QualityFailed", "Kalite Kontrolü Başarısız");
    public static readonly PrintItemStatus ReadyForDelivery = new(5, "ReadyForDelivery", "Teslimata Hazır");
    public static readonly PrintItemStatus Cancelled = new(6, "Cancelled", "İptal Edildi");

    private PrintItemStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanPrint => this == Pending || this == InProduction;
    public bool CanMarkReady => this == Printed;
    public bool CanCancel => this == Pending;
    public bool IsFinal => this == ReadyForDelivery || this == Cancelled;
}