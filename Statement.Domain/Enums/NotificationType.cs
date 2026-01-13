using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Enums;

/// <summary>
/// Bildirim Tipleri
/// </summary>
public class NotificationType : Enumeration
{
    public static readonly NotificationType Email = new(1, nameof(Email), "E-posta");
    public static readonly NotificationType SMS = new(2, nameof(SMS), "SMS");
    public static readonly NotificationType Push = new(3, nameof(Push), "Push Bildirim");
    public static readonly NotificationType Post = new(4, nameof(Post), "Posta");

    private NotificationType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}