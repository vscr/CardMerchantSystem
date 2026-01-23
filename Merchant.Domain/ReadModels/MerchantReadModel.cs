// Merchant.Domain/ReadModels/MerchantReadModel.cs

namespace Merchant.Domain.ReadModels;

/// <summary>
/// Dapper için sadece-okuma modeli
/// </summary>
public class MerchantReadModel
{
    public Guid Id { get; set; }
    public string MerchantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public int MerchantTypeId { get; set; }
    public int StatusId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string IBAN { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}