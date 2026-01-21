using CardMerchantSystem.Shared.Kernel;

namespace EarlyBlockResolution.Domain.Enums;

/// <summary>
/// Bloke durumları
/// </summary>
public class BlockStatus : Enumeration
{
    public static readonly BlockStatus Active = new(1, "Active", "Aktif");
    public static readonly BlockStatus PendingVerification = new(2, "PendingVerification", "Doğrulama Bekliyor");
    public static readonly BlockStatus Resolved = new(3, "Resolved", "Çözüldü");
    public static readonly BlockStatus Escalated = new(4, "Escalated", "Üst Seviyeye İletildi");
    public static readonly BlockStatus PermanentBlock = new(5, "PermanentBlock", "Kalıcı Bloke");
    public static readonly BlockStatus Expired = new(6, "Expired", "Süresi Doldu");

    private BlockStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool IsActive => this == Active || this == PendingVerification;
    public bool CanResolve => this == Active || this == PendingVerification;
    public bool CanEscalate => this == Active || this == PendingVerification;
    public bool IsFinal => this == Resolved || this == PermanentBlock || this == Expired;
}