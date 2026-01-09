namespace Dispute.Application.DTOs;

/// <summary>
/// İtiraz oluşturma request DTO
/// </summary>
public class CreateDisputeDto
{
    public Guid TransactionId { get; set; }
    public string TransactionReference { get; set; } = null!;
    public decimal TransactionAmount { get; set; }
    public decimal DisputedAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public int ReasonId { get; set; }
    public string Description { get; set; } = null!;
    public string? CustomerStatement { get; set; }

    // Müşteri Bilgileri
    public string CustomerTckn { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;

    // Üye İşyeri Bilgileri
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
}