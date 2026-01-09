using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Enums;

/// <summary>
/// İndirim hesaplama tipi
/// </summary>
public class DiscountType : Enumeration
{
    public static readonly DiscountType Percentage = new(1, nameof(Percentage), "Yüzde");
    public static readonly DiscountType FixedAmount = new(2, nameof(FixedAmount), "Sabit Tutar");

    private DiscountType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}