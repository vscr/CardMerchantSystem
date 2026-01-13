using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Enums;

/// <summary>
/// Ekstre Durumları
/// </summary>
public class StatementStatus : Enumeration
{
    public static readonly StatementStatus Draft = new(1, nameof(Draft), "Taslak");
    public static readonly StatementStatus Generated = new(2, nameof(Generated), "Oluşturuldu");
    public static readonly StatementStatus Sent = new(3, nameof(Sent), "Gönderildi");
    public static readonly StatementStatus Viewed = new(4, nameof(Viewed), "Görüntülendi");
    public static readonly StatementStatus PartiallyPaid = new(5, nameof(PartiallyPaid), "Kısmi Ödendi");
    public static readonly StatementStatus Paid = new(6, nameof(Paid), "Ödendi");
    public static readonly StatementStatus Overdue = new(7, nameof(Overdue), "Gecikmiş");
    public static readonly StatementStatus Cancelled = new(8, nameof(Cancelled), "İptal");

    private StatementStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool IsFinal => this == Paid || this == Cancelled;
    public bool RequiresPayment => this == Generated || this == Sent || this == Viewed || this == PartiallyPaid || this == Overdue;
}