using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Enums;

/// <summary>
/// Tarife Durumları
/// </summary>
public class TariffStatus : Enumeration
{
    public static readonly TariffStatus Draft = new(1, nameof(Draft), "Taslak");
    public static readonly TariffStatus Active = new(2, nameof(Active), "Aktif");
    public static readonly TariffStatus Suspended = new(3, nameof(Suspended), "Askıya Alınmış");
    public static readonly TariffStatus Expired = new(4, nameof(Expired), "Süresi Dolmuş");

    private TariffStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool IsUsable => this == Active;
}