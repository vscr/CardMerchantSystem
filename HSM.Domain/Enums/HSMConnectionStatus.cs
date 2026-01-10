using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Enums;

/// <summary>
/// HSM Bağlantı Durumları
/// </summary>
public class HSMConnectionStatus : Enumeration
{
    public static readonly HSMConnectionStatus Connected = new(1, nameof(Connected), "Bağlı");
    public static readonly HSMConnectionStatus Disconnected = new(2, nameof(Disconnected), "Bağlantı Kesildi");
    public static readonly HSMConnectionStatus Error = new(3, nameof(Error), "Hata");
    public static readonly HSMConnectionStatus Timeout = new(4, nameof(Timeout), "Zaman Aşımı");
    public static readonly HSMConnectionStatus Maintenance = new(5, nameof(Maintenance), "Bakımda");

    private HSMConnectionStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Kullanılabilir mi?
    /// </summary>
    public bool IsAvailable => this == Connected;
}