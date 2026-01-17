namespace MerchantSettlement.Application.DTOs;

public class SettlementDetailDto
{
    public Guid Id { get; set; }
    public Guid SettlementBatchId { get; set; }

    public string TransactionId { get; set; } = null!;
    public string TransactionNumber { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public DateTime TransactionDate { get; set; }

    public string CardNumberMasked { get; set; } = null!;
    public string CardBrand { get; set; } = null!;
    public string TerminalId { get; set; } = null!;

    public decimal Amount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = null!;

    public int InstallmentCount { get; set; }
    public string? OriginalTransactionId { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class AddSettlementDetailDto
{
    public string TransactionId { get; set; } = null!;
    public string TransactionNumber { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public string CardBrand { get; set; } = null!;
    public string TerminalId { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public int InstallmentCount { get; set; } = 1;
    public string? OriginalTransactionId { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ReferenceNumber { get; set; }
}