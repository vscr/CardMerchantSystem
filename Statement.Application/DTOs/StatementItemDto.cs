namespace Statement.Application.DTOs;

/// <summary>
/// Ekstre Kalemi DTO
/// </summary>
public class StatementItemDto
{
    public Guid Id { get; set; }
    public string ItemType { get; set; } = null!;
    public string ItemTypeDisplayName { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public DateTime PostDate { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? MerchantName { get; set; }
    public string? InstallmentInfo { get; set; }
    public string? OriginalCurrency { get; set; }
    public decimal? OriginalAmount { get; set; }
    public decimal? ExchangeRate { get; set; }
}

/// <summary>
/// Ekstre Kalemi Ekleme DTO
/// </summary>
public class AddStatementItemDto
{
    public Guid StatementId { get; set; }
    public int ItemTypeId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? MerchantName { get; set; }
    public int? InstallmentNumber { get; set; }
    public int? TotalInstallments { get; set; }
}