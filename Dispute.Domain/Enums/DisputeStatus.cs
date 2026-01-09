using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Enums;

/// <summary>
/// İtiraz durumları
/// </summary>
public class DisputeStatus : Enumeration
{
    public static readonly DisputeStatus Pending = new(1, nameof(Pending), "Beklemede");
    public static readonly DisputeStatus UnderReview = new(2, nameof(UnderReview), "İnceleniyor");
    public static readonly DisputeStatus InformationRequested = new(3, nameof(InformationRequested), "Bilgi Bekleniyor");
    public static readonly DisputeStatus AcceptedByMerchant = new(4, nameof(AcceptedByMerchant), "Üye İşyeri Kabul Etti");
    public static readonly DisputeStatus RejectedByMerchant = new(5, nameof(RejectedByMerchant), "Üye İşyeri Reddetti");
    public static readonly DisputeStatus EscalatedToBank = new(6, nameof(EscalatedToBank), "Bankaya Yönlendirildi");
    public static readonly DisputeStatus ResolvedInFavorOfCustomer = new(7, nameof(ResolvedInFavorOfCustomer), "Müşteri Lehine Çözüldü");
    public static readonly DisputeStatus ResolvedInFavorOfMerchant = new(8, nameof(ResolvedInFavorOfMerchant), "Üye İşyeri Lehine Çözüldü");
    public static readonly DisputeStatus Cancelled = new(9, nameof(Cancelled), "İptal Edildi");

    private DisputeStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Final durum mu?
    /// </summary>
    public bool IsFinal => this == ResolvedInFavorOfCustomer ||
                           this == ResolvedInFavorOfMerchant ||
                           this == Cancelled;

    /// <summary>
    /// Müşteri lehine mi?
    /// </summary>
    public bool IsInFavorOfCustomer => this == ResolvedInFavorOfCustomer ||
                                        this == AcceptedByMerchant;

    /// <summary>
    /// Geçiş yapılabilir durumlar
    /// </summary>
    public IReadOnlyList<DisputeStatus> AllowedTransitions
    {
        get
        {
            return this.Name switch
            {
                nameof(Pending) => new[] { UnderReview, Cancelled },
                nameof(UnderReview) => new[] { InformationRequested, AcceptedByMerchant, RejectedByMerchant, EscalatedToBank },
                nameof(InformationRequested) => new[] { UnderReview, Cancelled },
                nameof(AcceptedByMerchant) => new[] { ResolvedInFavorOfCustomer },
                nameof(RejectedByMerchant) => new[] { EscalatedToBank, ResolvedInFavorOfMerchant },
                nameof(EscalatedToBank) => new[] { ResolvedInFavorOfCustomer, ResolvedInFavorOfMerchant },
                _ => Array.Empty<DisputeStatus>()
            };
        }
    }

    public bool CanTransitionTo(DisputeStatus newStatus)
    {
        return AllowedTransitions.Contains(newStatus);
    }
}