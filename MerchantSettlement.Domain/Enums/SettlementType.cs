using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Enums;

/// <summary>
/// Takas tipleri
/// </summary>
public class SettlementType : Enumeration
{
    public static readonly SettlementType Daily = new(1, "Daily", "Günlük");
    public static readonly SettlementType Weekly = new(2, "Weekly", "Haftalık");
    public static readonly SettlementType Monthly = new(3, "Monthly", "Aylık");
    public static readonly SettlementType OnDemand = new(4, "OnDemand", "Talep Üzerine");

    private SettlementType(int id, string name, string displayName)
        : base(id, name, displayName) { }
}