using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Enums;

/// <summary>
/// Hakediş ödeme durumları
/// </summary>
public class PayoutStatus : Enumeration
{
    public static readonly PayoutStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly PayoutStatus Scheduled = new(2, "Scheduled", "Planlandı");
    public static readonly PayoutStatus Processing = new(3, "Processing", "İşleniyor");
    public static readonly PayoutStatus Paid = new(4, "Paid", "Ödendi");
    public static readonly PayoutStatus Failed = new(5, "Failed", "Başarısız");
    public static readonly PayoutStatus OnHold = new(6, "OnHold", "Bekletiliyor");
    public static readonly PayoutStatus Cancelled = new(7, "Cancelled", "İptal Edildi");

    private PayoutStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanSchedule => this == Pending;
    public bool CanProcess => this == Scheduled;
    public bool CanPay => this == Processing;
    public bool CanHold => this == Pending || this == Scheduled;
    public bool CanCancel => this == Pending || this == Scheduled || this == OnHold;
    public bool IsFinal => this == Paid || this == Cancelled;
}