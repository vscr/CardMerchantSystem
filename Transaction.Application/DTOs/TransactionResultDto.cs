namespace Transaction.Application.DTOs;

/// <summary>
/// İşlem sonucu DTO (POS cevabı)
/// </summary>
public class TransactionResultDto
{
    public bool IsApproved { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string ResponseCode { get; set; } = null!;
    public string ResponseMessage { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime TransactionTime { get; set; }
}