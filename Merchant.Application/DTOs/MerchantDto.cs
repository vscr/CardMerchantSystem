namespace Merchant.Application.DTOs;

/// <summary>
/// Üye işyeri response DTO
/// </summary>
public class MerchantDto
{
    public Guid Id { get; set; }
    public string MerchantCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string TradeName { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public string TaxOffice { get; set; } = null!;
    public string MerchantType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;

    // İletişim
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;

    // Banka
    public string IBAN { get; set; } = null!;

    // Sözleşme
    public decimal CommissionRate { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }

    // Onay
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Terminal sayısı
    public int ActiveTerminalCount { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}