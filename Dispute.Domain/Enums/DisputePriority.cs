using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Enums;

/// <summary>
/// İtiraz önceliği
/// </summary>
public class DisputePriority : Enumeration
{
    public static readonly DisputePriority Low = new(1, nameof(Low), "Düşük");
    public static readonly DisputePriority Normal = new(2, nameof(Normal), "Normal");
    public static readonly DisputePriority High = new(3, nameof(High), "Yüksek");
    public static readonly DisputePriority Critical = new(4, nameof(Critical), "Kritik");

    private DisputePriority(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Tutara göre öncelik belirle
    /// </summary>
    public static DisputePriority FromAmount(decimal amount)
    {
        return amount switch
        {
            > 50000 => Critical,
            > 10000 => High,
            > 1000 => Normal,
            _ => Low
        };
    }
}