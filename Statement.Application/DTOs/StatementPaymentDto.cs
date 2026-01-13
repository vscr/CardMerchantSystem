namespace Statement.Application.DTOs;

/// <summary>
/// Ekstre Ödeme DTO
/// </summary>
public class RecordStatementPaymentDto
{
    public Guid StatementId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentReference { get; set; }
}

/// <summary>
/// Ödeme Sonuç DTO
/// </summary>
public class StatementPaymentResultDto
{
    public Guid StatementId { get; set; }
    public string StatementNumber { get; set; } = null!;
    public decimal PaymentAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime PaymentDate { get; set; }
}