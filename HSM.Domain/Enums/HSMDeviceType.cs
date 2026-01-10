using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Enums;

/// <summary>
/// HSM Cihaz Tipleri
/// </summary>
public class HSMDeviceType : Enumeration
{
    public static readonly HSMDeviceType ThalesPayShield9000 = new(1, nameof(ThalesPayShield9000), "Thales PayShield 9000");
    public static readonly HSMDeviceType ThalesPayShield10K = new(2, nameof(ThalesPayShield10K), "Thales PayShield 10K");
    public static readonly HSMDeviceType GemaltoSafeNet = new(3, nameof(GemaltoSafeNet), "Gemalto SafeNet Luna");
    public static readonly HSMDeviceType Simulator = new(4, nameof(Simulator), "HSM Simülatör");

    private HSMDeviceType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Varsayılan port
    /// </summary>
    public int DefaultPort => this.Name switch
    {
        nameof(ThalesPayShield9000) => 1500,
        nameof(ThalesPayShield10K) => 1500,
        nameof(GemaltoSafeNet) => 1792,
        nameof(Simulator) => 9999,
        _ => 1500
    };
}