using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Application.DTOs;

/// <summary>
/// İtiraz listeleme filtre DTO'su
/// </summary>
public class DisputeFilterDto : PagedRequest
{
    /// <summary>
    /// İtiraz durumu filtresi
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Öncelik filtresi (1=Low, 2=Medium, 3=High, 4=Critical)
    /// </summary>
    public int? PriorityId { get; set; }

    /// <summary>
    /// İtiraz nedeni filtresi
    /// </summary>
    public int? ReasonId { get; set; }

    /// <summary>
    /// Üye işyeri ID filtresi
    /// </summary>
    public Guid? MerchantId { get; set; }

    /// <summary>
    /// Müşteri TCKN filtresi
    /// </summary>
    public string? CustomerTckn { get; set; }

    /// <summary>
    /// İşlem ID filtresi
    /// </summary>
    public Guid? TransactionId { get; set; }

    /// <summary>
    /// Atanan kişi filtresi
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Sadece vadesi geçmişleri getir
    /// </summary>
    public bool? IsOverdue { get; set; }

    /// <summary>
    /// Arama terimi (itiraz numarası, müşteri adı, merchant kodu)
    /// </summary>
    public string? SearchTerm { get; set; }
}