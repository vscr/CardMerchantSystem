namespace BKM.Application.DTOs;

/// <summary>
/// Clearing Record DTO
/// </summary>
public class ClearingRecordDto
{
    public Guid Id { get; set; }
    public Guid SwitchMessageId { get; set; }
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;
    public string AuthorizationCode { get; set; } = null!;

    // İşlem Bilgileri
    public decimal TransactionAmount { get; set; }
    public decimal ClearingAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = null!;

    // Taraflar
    public string AcquirerBankCode { get; set; } = null!;
    public string IssuerBankCode { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    // Kart Bilgileri
    public string CardNumberMasked { get; set; } = null!;
    public string BIN { get; set; } = null!;

    // Tarihler
    public DateTime TransactionDate { get; set; }
    public string ClearingDate { get; set; } = null!;
    public bool IsSettled { get; set; }
    public DateTime? SettledAt { get; set; }
}