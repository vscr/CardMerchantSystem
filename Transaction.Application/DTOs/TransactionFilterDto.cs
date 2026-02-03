using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Application.DTOs;

/// <summary>
/// İşlem listeleme filtre DTO'su
/// </summary>
public class TransactionFilterDto : PagedRequest
{
    /// <summary>
    /// İşlem durumu filtresi (1=Pending, 2=Approved, 3=Declined, 4=Settled, 5=Refunded, 6=Cancelled)
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// İşlem tipi filtresi (1=Sale, 2=Refund, 3=Void, 4=PreAuth, 5=PostAuth)
    /// </summary>
    public int? TransactionTypeId { get; set; }

    /// <summary>
    /// Üye işyeri ID filtresi
    /// </summary>
    public Guid? MerchantId { get; set; }

    /// <summary>
    /// Terminal ID filtresi
    /// </summary>
    public Guid? TerminalId { get; set; }

    /// <summary>
    /// Maskelenmiş kart numarası
    /// </summary>
    public string? CardNumberMasked { get; set; }

    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Minimum tutar
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Maksimum tutar
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Referans numarası ile arama
    /// </summary>
    public string? ReferenceNumber { get; set; }
}