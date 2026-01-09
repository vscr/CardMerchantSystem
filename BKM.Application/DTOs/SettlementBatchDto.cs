namespace BKM.Application.DTOs;

/// <summary>
/// Settlement Batch DTO
/// </summary>
public class SettlementBatchDto
{
    public Guid Id { get; set; }
    public string SettlementDate { get; set; } = null!;
    public string BatchNumber { get; set; } = null!;

    // Özet Bilgiler
    public int TotalTransactionCount { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public decimal TotalFeeAmount { get; set; }
    public decimal TotalNetAmount { get; set; }

    // Durum
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Banka Özetleri
    public List<BankSettlementSummaryDto> BankSummaries { get; set; } = new();
}

/// <summary>
/// Bank Settlement Summary DTO
/// </summary>
public class BankSettlementSummaryDto
{
    public Guid Id { get; set; }
    public string BankCode { get; set; } = null!;
    public bool IsAcquirer { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalFee { get; set; }
    public decimal NetAmount { get; set; }
}