using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Application.DTOs;

/// <summary>
/// Kampanya listeleme filtre DTO'su
/// </summary>
public class CampaignFilterDto : PagedRequest
{
    /// <summary>
    /// Kampanya durumu filtresi (1=Draft, 2=Pending, 3=Active, 4=Paused, 5=Completed, 6=Cancelled)
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Kampanya tipi filtresi
    /// </summary>
    public int? CampaignTypeId { get; set; }

    /// <summary>
    /// İndirim tipi filtresi
    /// </summary>
    public int? DiscountTypeId { get; set; }

    /// <summary>
    /// Hedef kitle filtresi
    /// </summary>
    public int? TargetAudienceId { get; set; }

    /// <summary>
    /// Üye işyeri ID filtresi (bu merchant'a uygulanabilir kampanyalar)
    /// </summary>
    public Guid? MerchantId { get; set; }

    /// <summary>
    /// Başlangıç tarihi başlangıcı
    /// </summary>
    public DateTime? StartDateFrom { get; set; }

    /// <summary>
    /// Başlangıç tarihi bitişi
    /// </summary>
    public DateTime? StartDateTo { get; set; }

    /// <summary>
    /// Sadece şu an aktif olanları getir
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Arama terimi (kampanya adı, kodu, açıklama)
    /// </summary>
    public string? SearchTerm { get; set; }
}