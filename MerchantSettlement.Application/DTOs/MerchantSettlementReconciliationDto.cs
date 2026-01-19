namespace MerchantSettlement.Application.DTOs;

public class MerchantSettlementReconciliationDto
{
    public Guid Id { get; set; }
    public string ReconciliationNumber { get; set; } = null!;
    public Guid SettlementBatchId { get; set; }
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;

    public DateTime ReconciliationDate { get; set; }

    public decimal SystemGrossAmount { get; set; }
    public decimal SystemCommission { get; set; }
    public decimal SystemNetAmount { get; set; }
    public int SystemTransactionCount { get; set; }

    public decimal ReportedGrossAmount { get; set; }
    public decimal ReportedCommission { get; set; }
    public decimal ReportedNetAmount { get; set; }
    public int ReportedTransactionCount { get; set; }

    public decimal GrossAmountDifference { get; set; }
    public decimal CommissionDifference { get; set; }
    public decimal NetAmountDifference { get; set; }
    public int TransactionCountDifference { get; set; }

    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;

    public string? ResolutionNotes { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class MerchantSettlementReconciliationWithMismatchesDto : MerchantSettlementReconciliationDto
{
    public List<MerchantReconciliationMismatchDto> Mismatches { get; set; } = new();
}

public class MerchantReconciliationMismatchDto
{
    public Guid Id { get; set; }
    public Guid ReconciliationId { get; set; }

    public string? TransactionId { get; set; }
    public string? TransactionNumber { get; set; }
    public DateTime? TransactionDate { get; set; }

    public string MismatchType { get; set; } = null!;

    public decimal? SystemAmount { get; set; }
    public decimal? ReportedAmount { get; set; }
    public decimal? AmountDifference { get; set; }

    public string? Description { get; set; }

    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class SetReportedAmountsDto
{
    public Guid ReconciliationId { get; set; }
    public decimal ReportedGrossAmount { get; set; }
    public decimal ReportedCommission { get; set; }
    public decimal ReportedNetAmount { get; set; }
    public int ReportedTransactionCount { get; set; }
}

public class ResolveReconciliationDto
{
    public Guid ReconciliationId { get; set; }
    public string Notes { get; set; } = null!;
}