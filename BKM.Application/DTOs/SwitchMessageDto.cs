namespace BKM.Application.DTOs;

/// <summary>
/// Switch Message DTO
/// </summary>
public class SwitchMessageDto
{
    public Guid Id { get; set; }
    public string MessageType { get; set; } = null!;
    public string MessageTypeDisplayName { get; set; } = null!;
    public string ProcessingCode { get; set; } = null!;
    public string ProcessingCodeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;

    // Kart Bilgileri
    public string CardNumberMasked { get; set; } = null!;
    public string BIN { get; set; } = null!;

    // İşlem Bilgileri
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime TransactionDateTime { get; set; }

    // Terminal/Üye İşyeri
    public string TerminalId { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MCC { get; set; } = null!;

    // Bankalar
    public string AcquirerBankCode { get; set; } = null!;
    public string IssuerBankCode { get; set; } = null!;

    // Response
    public string? ResponseCode { get; set; }
    public string? ResponseCodeDisplayName { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ErrorMessage { get; set; }

    // Timing
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public int ProcessingTimeMs { get; set; }
}