using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Enums;

/// <summary>
/// Kart başvuru durumları.
/// State machine mantığı ile çalışır.
/// 
/// Flow: Pending -> UnderReview -> Approved/Rejected
///       Approved -> CardRequested -> CardPrinted -> ReadyForDelivery -> InDelivery -> Delivered
/// </summary>
public class CardApplicationStatus : Enumeration
{
    // Başvuru aşaması
    public static readonly CardApplicationStatus Pending = new(1, nameof(Pending), "Beklemede");
    public static readonly CardApplicationStatus UnderReview = new(2, nameof(UnderReview), "İnceleniyor");
    public static readonly CardApplicationStatus Approved = new(3, nameof(Approved), "Onaylandı");
    public static readonly CardApplicationStatus Rejected = new(4, nameof(Rejected), "Reddedildi");

    // Kart basım aşaması
    public static readonly CardApplicationStatus CardRequested = new(5, nameof(CardRequested), "Kart Talep Edildi");
    public static readonly CardApplicationStatus CardPrinted = new(6, nameof(CardPrinted), "Kart Basıldı");

    // Teslimat aşaması
    public static readonly CardApplicationStatus ReadyForDelivery = new(7, nameof(ReadyForDelivery), "Teslimata Hazır");
    public static readonly CardApplicationStatus InDelivery = new(8, nameof(InDelivery), "Teslimat Sürecinde");
    public static readonly CardApplicationStatus Delivered = new(9, nameof(Delivered), "Teslim Edildi");

    // İptal
    public static readonly CardApplicationStatus Cancelled = new(10, nameof(Cancelled), "İptal Edildi");

    private CardApplicationStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    private static readonly Dictionary<CardApplicationStatus, List<CardApplicationStatus>> _allowedTransitions = new()
    {
        { Pending, new List<CardApplicationStatus> { UnderReview, Cancelled } },
        { UnderReview, new List<CardApplicationStatus> { Approved, Rejected } },
        { Approved, new List<CardApplicationStatus> { CardRequested, Cancelled } },
        { Rejected, new List<CardApplicationStatus>() },
        { CardRequested, new List<CardApplicationStatus> { CardPrinted, Cancelled } },
        { CardPrinted, new List<CardApplicationStatus> { ReadyForDelivery } },
        { ReadyForDelivery, new List<CardApplicationStatus> { InDelivery } },
        { InDelivery, new List<CardApplicationStatus> { Delivered } },
        { Delivered, new List<CardApplicationStatus>() },
        { Cancelled, new List<CardApplicationStatus>() }
    };

    public IReadOnlyCollection<CardApplicationStatus> AllowedTransitions => _allowedTransitions[this];

    public bool CanTransitionTo(CardApplicationStatus newStatus)
    {
        return AllowedTransitions.Contains(newStatus);
    }

    public bool IsFinalState => !AllowedTransitions.Any();

    public bool IsCancellable => AllowedTransitions.Contains(Cancelled);
}