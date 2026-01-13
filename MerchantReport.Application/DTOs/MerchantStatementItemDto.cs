namespace MerchantReport.Application.DTOs;

/// <summary>
/// Üye İşyeri Ekstre Kalemi DTO
/// </summary>
public class MerchantStatementItemDto
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = null!;
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CardNumber { get; set; }
    public string? TerminalId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int InstallmentCount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Ekstre Kalemi Ekleme DTO
/// </summary>
public class AddMerchantStatementItemDto
{
    public Guid StatementId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = null!;
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CardNumber { get; set; }
    public string? TerminalId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int InstallmentCount { get; set; } = 1;
    public string? Description { get; set; }
}