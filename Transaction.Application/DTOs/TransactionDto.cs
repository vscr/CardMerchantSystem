namespace Transaction.Application.DTOs;

/// <summary>
/// İşlem response DTO
/// </summary>
public class TransactionDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public string TransactionTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int StatusId { get; set; }
    public string StatusDisplayName { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public string TerminalCode { get; set; } = null!;
    public string? DeclineReason { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FraudCheckResult { get; set; }
    public int? FraudScore { get; set; }
    public Guid? OriginalTransactionId { get; set; }
    public DateTime? SettledAt { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}