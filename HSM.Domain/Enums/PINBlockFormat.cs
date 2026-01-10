using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Enums;

/// <summary>
/// PIN Block Formatları
/// </summary>
public class PINBlockFormat : Enumeration
{
    public static readonly PINBlockFormat ISO0 = new(1, "01", "ISO Format 0 (ANSI)");
    public static readonly PINBlockFormat ISO1 = new(2, "02", "ISO Format 1");
    public static readonly PINBlockFormat ISO2 = new(3, "03", "ISO Format 2");
    public static readonly PINBlockFormat ISO3 = new(4, "04", "ISO Format 3");
    public static readonly PINBlockFormat ISO4 = new(5, "05", "ISO Format 4 (AES)");
    public static readonly PINBlockFormat ANSI = new(6, "01", "ANSI X9.8");
    public static readonly PINBlockFormat Diebold = new(7, "34", "Diebold Format");

    private PINBlockFormat(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}