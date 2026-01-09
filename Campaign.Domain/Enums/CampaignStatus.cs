using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Enums;

/// <summary>
/// Kampanya durumları
/// </summary>
public class CampaignStatus : Enumeration
{
    public static readonly CampaignStatus Draft = new(1, nameof(Draft), "Taslak");
    public static readonly CampaignStatus Pending = new(2, nameof(Pending), "Onay Bekliyor");
    public static readonly CampaignStatus Active = new(3, nameof(Active), "Aktif");
    public static readonly CampaignStatus Paused = new(4, nameof(Paused), "Duraklatıldı");
    public static readonly CampaignStatus Completed = new(5, nameof(Completed), "Tamamlandı");
    public static readonly CampaignStatus Cancelled = new(6, nameof(Cancelled), "İptal Edildi");

    private CampaignStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Kampanya kullanılabilir mi?
    /// </summary>
    public bool IsUsable => this == Active;

    /// <summary>
    /// Final durum mu?
    /// </summary>
    public bool IsFinal => this == Completed || this == Cancelled;

    /// <summary>
    /// Geçiş yapılabilir durumlar
    /// </summary>
    public IReadOnlyList<CampaignStatus> AllowedTransitions
    {
        get
        {
            return this.Name switch
            {
                nameof(Draft) => new[] { Pending, Cancelled },
                nameof(Pending) => new[] { Active, Draft, Cancelled },
                nameof(Active) => new[] { Paused, Completed, Cancelled },
                nameof(Paused) => new[] { Active, Cancelled },
                _ => Array.Empty<CampaignStatus>()
            };
        }
    }

    public bool CanTransitionTo(CampaignStatus newStatus)
    {
        return AllowedTransitions.Contains(newStatus);
    }
}