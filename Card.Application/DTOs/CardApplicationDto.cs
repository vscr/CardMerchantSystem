using Card.Domain.Entities;

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
    public int CardTypeId { get; set; }
    public string CardType { get; set; } = null!;
    public int StatusId { get; set; }
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

    /// <summary>
    /// Entity'den DTO'ya dönüşüm
    /// </summary>
    public static CardApplicationDto FromEntity(CardApplication entity)
    {
        return new CardApplicationDto
        {
            Id = entity.Id,
            CustomerTckn = entity.CustomerTckn.Value,
            CustomerName = entity.CustomerName,
            CustomerSurname = entity.CustomerSurname,
            CustomerFullName = entity.CustomerFullName,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            DeliveryAddress = entity.DeliveryAddress.SingleLine,
            CardTypeId = entity.CardType.Id,
            CardType = entity.CardType.Name,
            StatusId = entity.Status.Id,
            Status = entity.Status.Name,
            StatusDisplayName = entity.Status.DisplayName,
            CardNumberMasked = entity.CardNumberMasked,
            DailyLimit = entity.DailyLimit.Amount,
            MonthlyLimit = entity.MonthlyLimit.Amount,
            Currency = entity.DailyLimit.Currency,
            PrintVendor = entity.PrintVendor?.DisplayName,
            PrintBatchId = entity.PrintBatchId,
            PrintedAt = entity.PrintedAt,
            CourierTrackingNumber = entity.CourierTrackingNumber,
            DeliveredAt = entity.DeliveredAt,
            ApprovedBy = entity.ApprovedBy,
            ApprovedAt = entity.ApprovedAt,
            RejectionReason = entity.RejectionReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}