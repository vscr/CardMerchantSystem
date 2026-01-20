namespace MerchantSettlement.Application.DTOs;

public class MerchantPayoutDto
{
    public Guid Id { get; set; }
    public string PayoutNumber { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;

    public string BankCode { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string Iban { get; set; } = null!;

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public decimal GrossAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalFee { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;

    public DateTime? ScheduledDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? BankReferenceNumber { get; set; }

    public string? HoldReason { get; set; }
    public string? FailureReason { get; set; }

    public List<Guid> SettlementBatchIds { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}

public class CreateMerchantPayoutDto
{
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public string BankCode { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<Guid> SettlementBatchIds { get; set; } = new();
    public decimal WithholdingTaxRate { get; set; } = 0;
}

public class SchedulePayoutDto
{
    public Guid PayoutId { get; set; }
    public DateTime ScheduledDate { get; set; }
}

public class CompletePayoutDto
{
    public Guid PayoutId { get; set; }
    public string BankReferenceNumber { get; set; } = null!;
}