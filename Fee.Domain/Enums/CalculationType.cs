using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Enums;

/// <summary>
/// Hesaplama Tipleri
/// </summary>
public class CalculationType : Enumeration
{
    public static readonly CalculationType Percentage = new(1, nameof(Percentage), "Yüzde");
    public static readonly CalculationType FixedAmount = new(2, nameof(FixedAmount), "Sabit Tutar");
    public static readonly CalculationType PercentageWithMinimum = new(3, nameof(PercentageWithMinimum), "Yüzde + Minimum");
    public static readonly CalculationType PercentageWithMaximum = new(4, nameof(PercentageWithMaximum), "Yüzde + Maksimum");
    public static readonly CalculationType Tiered = new(5, nameof(Tiered), "Kademeli");

    private CalculationType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}