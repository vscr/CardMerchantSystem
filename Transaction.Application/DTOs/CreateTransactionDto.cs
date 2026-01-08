namespace Transaction.Application.DTOs;

/// <summary>
/// İşlem oluşturma request DTO
/// </summary>
public class CreateTransactionDto
{
    public int TransactionTypeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string CardNumberMasked { get; set; } = null!;
    public string CardNumberEncrypted { get; set; } = null!;
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public string TerminalCode { get; set; } = null!;
    public Guid? OriginalTransactionId { get; set; }
}