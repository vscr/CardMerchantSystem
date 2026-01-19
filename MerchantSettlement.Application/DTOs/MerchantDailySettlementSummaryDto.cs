namespace MerchantSettlement.Application.DTOs;

public class MerchantDailySettlementSummaryDto
{
    public Guid Id { get; set; }
    public DateTime SettlementDate { get; set; }

    public int TotalMerchantCount { get; set; }
    public int TotalBatchCount { get; set; }
    public int TotalTransactionCount { get; set; }

    public decimal TotalSalesAmount { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal TotalChargebackAmount { get; set; }
    public decimal TotalGrossAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalFee { get; set; }
    public decimal TotalNetAmount { get; set; }
    public string Currency { get; set; } = null!;

    public int SalesCount { get; set; }
    public int RefundCount { get; set; }
    public int ChargebackCount { get; set; }

    public int CompletedBatchCount { get; set; }
    public int FailedBatchCount { get; set; }
    public int PendingBatchCount { get; set; }

    public bool IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public string? FinalizedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}