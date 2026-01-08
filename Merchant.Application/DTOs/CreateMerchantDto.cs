namespace Merchant.Application.DTOs;

/// <summary>
/// Üye işyeri oluşturma request DTO
/// </summary>
public class CreateMerchantDto
{
    public string Name { get; set; } = null!;
    public string TradeName { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public string TaxOffice { get; set; } = null!;
    public int MerchantTypeId { get; set; }

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
}