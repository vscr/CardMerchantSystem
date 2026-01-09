using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Enums;

/// <summary>
/// Hedef kitle
/// </summary>
public class TargetAudience : Enumeration
{
    public static readonly TargetAudience All = new(1, nameof(All), "Tüm Müşteriler");
    public static readonly TargetAudience NewCustomers = new(2, nameof(NewCustomers), "Yeni Müşteriler");
    public static readonly TargetAudience ExistingCustomers = new(3, nameof(ExistingCustomers), "Mevcut Müşteriler");
    public static readonly TargetAudience PremiumCustomers = new(4, nameof(PremiumCustomers), "Premium Müşteriler");
    public static readonly TargetAudience SelectedMerchants = new(5, nameof(SelectedMerchants), "Seçili Üye İşyerleri");
    public static readonly TargetAudience SelectedCards = new(6, nameof(SelectedCards), "Seçili Kartlar");

    private TargetAudience(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}