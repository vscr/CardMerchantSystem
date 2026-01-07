using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Enums;

/// <summary>
/// Kart basım vendorları.
/// İş ilanındaki: Bileşim, Austuria, Evolis Primacy
/// </summary>
public class PrintVendor : Enumeration
{
    public static readonly PrintVendor Bilesim = new(1, nameof(Bilesim), "Bileşim Kart",
        isBatchPrint: true, supportsInstantPrint: false);

    public static readonly PrintVendor Austuria = new(2, nameof(Austuria), "Austuria Kart",
        isBatchPrint: true, supportsInstantPrint: false);

    public static readonly PrintVendor EvolisPrimacy = new(3, nameof(EvolisPrimacy), "Evolis Primacy",
        isBatchPrint: false, supportsInstantPrint: true);

    /// <summary>
    /// Toplu basım yapabilir mi?
    /// </summary>
    public bool IsBatchPrint { get; }

    /// <summary>
    /// Anında (şube içi) basım yapabilir mi?
    /// </summary>
    public bool SupportsInstantPrint { get; }

    private PrintVendor(int id, string name, string displayName,
        bool isBatchPrint, bool supportsInstantPrint)
        : base(id, name, displayName)
    {
        IsBatchPrint = isBatchPrint;
        SupportsInstantPrint = supportsInstantPrint;
    }
}