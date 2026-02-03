namespace Dispute.Application.DTOs;

/// <summary>
/// İtiraz response DTO
/// </summary>
public class DisputeDto
{
    public Guid Id { get; set; }
    public string DisputeNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int StatusId { get; set; }
    public string StatusDisplayName { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public int ReasonId { get; set; }
    public string ReasonDisplayName { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public int PriorityId { get; set; }
    public string PriorityDisplayName { get; set; } = null!;

    // İşlem Bilgileri
    public Guid TransactionId { get; set; }
    public string TransactionReference { get; set; } = null!;
    public decimal TransactionAmount { get; set; }
    public decimal DisputedAmount { get; set; }
    public DateTime TransactionDate { get; set; }

    // Müşteri Bilgileri
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;

    // Üye İşyeri Bilgileri
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public string MerchantName { get; set; } = null!;

    // Detaylar
    public string Description { get; set; } = null!;
    public string? CustomerStatement { get; set; }
    public string? MerchantResponse { get; set; }
    public DateTime? MerchantResponseDate { get; set; }

    // Atama ve Çözüm
    public string? AssignedTo { get; set; }
    public DateTime? AssignedAt { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public decimal? RefundAmount { get; set; }

    // Tarihler
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOverdue { get; set; }
    public int RemainingDays { get; set; }

    // Koleksiyonlar
    public int DocumentCount { get; set; }
    public int NoteCount { get; set; }
}