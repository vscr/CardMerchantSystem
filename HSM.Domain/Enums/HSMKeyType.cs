using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Enums;

/// <summary>
/// HSM Key Tipleri
/// </summary>
public class HSMKeyType : Enumeration
{
    public static readonly HSMKeyType ZMK = new(1, "000", "Zone Master Key");
    public static readonly HSMKeyType ZPK = new(2, "001", "Zone PIN Key");
    public static readonly HSMKeyType TMK = new(3, "002", "Terminal Master Key");
    public static readonly HSMKeyType TPK = new(4, "003", "Terminal PIN Key");
    public static readonly HSMKeyType TAK = new(5, "004", "Terminal Authentication Key");
    public static readonly HSMKeyType PVK = new(6, "005", "PIN Verification Key");
    public static readonly HSMKeyType CVK = new(7, "402", "Card Verification Key");
    public static readonly HSMKeyType MDK = new(8, "109", "Master Derivation Key");
    public static readonly HSMKeyType DEK = new(9, "00A", "Data Encryption Key");
    public static readonly HSMKeyType MAC = new(10, "003", "MAC Key");

    private HSMKeyType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}