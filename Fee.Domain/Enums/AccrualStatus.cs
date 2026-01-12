using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Enums;

/// <summary>
/// Tahakkuk Durumları
/// </summary>
public class AccrualStatus : Enumeration
{
    public static readonly AccrualStatus Pending = new(1, nameof(Pending), "Bekliyor");
    public static readonly AccrualStatus Invoiced = new(2, nameof(Invoiced), "Faturalandı");
    public static readonly AccrualStatus PartiallyPaid = new(3, nameof(PartiallyPaid), "Kısmi Ödendi");
    public static readonly AccrualStatus Paid = new(4, nameof(Paid), "Ödendi");
    public static readonly AccrualStatus Cancelled = new(5, nameof(Cancelled), "İptal Edildi");
    public static readonly AccrualStatus Waived = new(6, nameof(Waived), "Muaf Tutuldu");

    private AccrualStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool IsFinal => this == Paid || this == Cancelled || this == Waived;
    public bool RequiresPayment => this == Pending || this == Invoiced || this == PartiallyPaid;
}