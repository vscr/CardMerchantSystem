using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Enums;

/// <summary>
/// Üye işyeri durumları
/// </summary>
public class MerchantStatus : Enumeration
{
    public static readonly MerchantStatus Pending = new(1, nameof(Pending), "Başvuru Bekliyor");
    public static readonly MerchantStatus UnderReview = new(2, nameof(UnderReview), "İnceleniyor");
    public static readonly MerchantStatus Approved = new(3, nameof(Approved), "Onaylandı");
    public static readonly MerchantStatus Active = new(4, nameof(Active), "Aktif");
    public static readonly MerchantStatus Suspended = new(5, nameof(Suspended), "Askıya Alındı");
    public static readonly MerchantStatus Closed = new(6, nameof(Closed), "Kapatıldı");
    public static readonly MerchantStatus Rejected = new(7, nameof(Rejected), "Reddedildi");

    private MerchantStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    private static readonly Dictionary<MerchantStatus, List<MerchantStatus>> _allowedTransitions = new()
    {
        { Pending, new List<MerchantStatus> { UnderReview, Rejected } },
        { UnderReview, new List<MerchantStatus> { Approved, Rejected } },
        { Approved, new List<MerchantStatus> { Active } },
        { Active, new List<MerchantStatus> { Suspended, Closed } },
        { Suspended, new List<MerchantStatus> { Active, Closed } },
        { Closed, new List<MerchantStatus>() },
        { Rejected, new List<MerchantStatus>() }
    };

    public IReadOnlyCollection<MerchantStatus> AllowedTransitions => _allowedTransitions[this];

    public bool CanTransitionTo(MerchantStatus newStatus)
    {
        return AllowedTransitions.Contains(newStatus);
    }

    public bool CanProcessTransactions => this == Active;
}