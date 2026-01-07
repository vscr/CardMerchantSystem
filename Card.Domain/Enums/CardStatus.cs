using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Enums;

/// <summary>
/// Kart durumları (aktif kartlar için)
/// </summary>
public class CardStatus : Enumeration
{
    public static readonly CardStatus Inactive = new(1, nameof(Inactive), "Aktif Değil");
    public static readonly CardStatus Active = new(2, nameof(Active), "Aktif");
    public static readonly CardStatus Blocked = new(3, nameof(Blocked), "Blokeli");
    public static readonly CardStatus Suspended = new(4, nameof(Suspended), "Askıya Alındı");
    public static readonly CardStatus Expired = new(5, nameof(Expired), "Süresi Doldu");
    public static readonly CardStatus Cancelled = new(6, nameof(Cancelled), "İptal Edildi");
    public static readonly CardStatus Lost = new(7, nameof(Lost), "Kayıp");
    public static readonly CardStatus Stolen = new(8, nameof(Stolen), "Çalıntı");

    private CardStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Bu kartla işlem yapılabilir mi?
    /// </summary>
    public bool CanPerformTransaction => this == Active;

    /// <summary>
    /// Bu kart yeniden aktif edilebilir mi?
    /// </summary>
    public bool CanBeReactivated => this == Suspended || this == Blocked;

    /// <summary>
    /// Bu kart kalıcı olarak devre dışı mı?
    /// </summary>
    public bool IsPermanentlyDisabled => this == Cancelled || this == Lost || this == Stolen;
}