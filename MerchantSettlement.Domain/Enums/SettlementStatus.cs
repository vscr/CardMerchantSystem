using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Enums;

/// <summary>
/// Günsonu batch durumları
/// </summary>
public class SettlementStatus : Enumeration
{
    public static readonly SettlementStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly SettlementStatus Processing = new(2, "Processing", "İşleniyor");
    public static readonly SettlementStatus Completed = new(3, "Completed", "Tamamlandı");
    public static readonly SettlementStatus Failed = new(4, "Failed", "Başarısız");
    public static readonly SettlementStatus Cancelled = new(5, "Cancelled", "İptal Edildi");
    public static readonly SettlementStatus PartiallyCompleted = new(6, "PartiallyCompleted", "Kısmen Tamamlandı");

    private SettlementStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanProcess => this == Pending;
    public bool CanCancel => this == Pending || this == Failed;
    public bool IsFinal => this == Completed || this == Cancelled;
}