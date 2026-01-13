using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Enums;

/// <summary>
/// Ödeme Durumları
/// </summary>
public class PaymentStatus : Enumeration
{
    public static readonly PaymentStatus Current = new(1, nameof(Current), "Güncel");
    public static readonly PaymentStatus DueSoon = new(2, nameof(DueSoon), "Yaklaşan Vade");
    public static readonly PaymentStatus Overdue1To30 = new(3, nameof(Overdue1To30), "1-30 Gün Gecikmiş");
    public static readonly PaymentStatus Overdue31To60 = new(4, nameof(Overdue31To60), "31-60 Gün Gecikmiş");
    public static readonly PaymentStatus Overdue61To90 = new(5, nameof(Overdue61To90), "61-90 Gün Gecikmiş");
    public static readonly PaymentStatus Overdue90Plus = new(6, nameof(Overdue90Plus), "90+ Gün Gecikmiş");

    private PaymentStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public static PaymentStatus FromDaysOverdue(int days)
    {
        return days switch
        {
            <= 0 => Current,
            <= 7 => DueSoon,
            <= 30 => Overdue1To30,
            <= 60 => Overdue31To60,
            <= 90 => Overdue61To90,
            _ => Overdue90Plus
        };
    }

    public bool IsOverdue => this != Current && this != DueSoon;
}