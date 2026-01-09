namespace BKM.Application.DTOs;

/// <summary>
/// Authorization Request DTO
/// </summary>
public class AuthorizationRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string TerminalId { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MCC { get; set; } = null!;
    public string AcquirerBankCode { get; set; } = null!;
}

/// <summary>
/// Authorization Response DTO
/// </summary>
public class AuthorizationResponseDto
{
    public bool IsApproved { get; set; }
    public string ResponseCode { get; set; } = null!;
    public string ResponseMessage { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;
    public int ProcessingTimeMs { get; set; }
    public Guid MessageId { get; set; }
}