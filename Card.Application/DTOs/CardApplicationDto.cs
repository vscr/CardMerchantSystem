namespace Card.Application.DTOs;

/// <summary>
/// Kart başvurusu response DTO
/// </summary>
public class CardApplicationDto
{
    public Guid Id { get; set; }
    public string CustomerTckn { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerSurname { get; set; } = null!;
    public string CustomerFullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Adres
    public string DeliveryAddress { get; set; } = null!;

    // Kart Bilgileri
    public string CardType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string? CardNumberMasked { get; set; }

    // Limitler
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public string Currency { get; set; } = null!;

    // Basım
    public string? PrintVendor { get; set; }
    public string? PrintBatchId { get; set; }
    public DateTime? PrintedAt { get; set; }

    // Teslimat
    public string? CourierTrackingNumber { get; set; }
    public DateTime? DeliveredAt { get; set; }

    // Onay/Red
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}