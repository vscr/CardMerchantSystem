namespace MerchantSettlement.Application.DTOs;

public class MerchantSettlementBatchDto
{
    public Guid Id { get; set; }
    public string BatchNumber { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string SettlementType { get; set; } = null!;
    public string SettlementTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;

    public decimal TotalSalesAmount { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal TotalChargebackAmount { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalFee { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = null!;

    public int SalesCount { get; set; }
    public int RefundCount { get; set; }
    public int ChargebackCount { get; set; }
    public int TotalTransactionCount { get; set; }

    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class MerchantSettlementBatchWithDetailsDto : MerchantSettlementBatchDto
{
    public List<MerchantSettlementDetailDto> Details { get; set; } = new();
}

public class CreateMerchantSettlementBatchDto
{
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int SettlementTypeId { get; set; }
}