using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Application.DTOs;

/// <summary>
/// Üye işyeri listeleme filtre DTO'su
/// </summary>
public class MerchantFilterDto : PagedRequest
{
    /// <summary>
    /// Durum filtresi (opsiyonel)
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Arama terimi (isim, ticari unvan, merchant code, email)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Üye işyeri tipi filtresi
    /// </summary>
    public int? MerchantTypeId { get; set; }

    /// <summary>
    /// Şehir filtresi
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi
    /// </summary>
    public DateTime? EndDate { get; set; }
}