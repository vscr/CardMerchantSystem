using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Enums;

/// <summary>
/// Terminal tipleri
/// İş ilanındaki: Beko, Ingenico, Teknoser
/// </summary>
public class TerminalType : Enumeration
{
    public static readonly TerminalType Ingenico = new(1, nameof(Ingenico), "Ingenico POS");
    public static readonly TerminalType Beko = new(2, nameof(Beko), "Beko POS");
    public static readonly TerminalType Teknoser = new(3, nameof(Teknoser), "Teknoser POS");
    public static readonly TerminalType VirtualPOS = new(4, nameof(VirtualPOS), "Sanal POS");
    public static readonly TerminalType MobilePOS = new(5, nameof(MobilePOS), "Mobil POS");

    private TerminalType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Fiziksel cihaz mı?
    /// </summary>
    public bool IsPhysical => this != VirtualPOS;

    /// <summary>
    /// Temassız ödeme destekliyor mu?
    /// </summary>
    public bool SupportsContactless => this == Ingenico || this == Beko || this == MobilePOS;
}