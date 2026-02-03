namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Dapper okuma işlemleri için hafif DTO
/// Entity'ye dönüşüm gerektirmez, direkt kullanılır
/// </summary>
public class TransactionReadDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public int StatusId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public string TerminalCode { get; set; } = null!;
    public int? DeclineReasonId { get; set; }
    public string? ErrorMessage { get; set; }
    public int? FraudCheckResultId { get; set; }
    public int? FraudScore { get; set; }
    public Guid? OriginalTransactionId { get; set; }
    public DateTime? SettledAt { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// İşlem istatistikleri DTO (Dashboard için)
/// </summary>
public class TransactionStatsReadDto
{
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
    public int SuccessfulCount { get; set; }
    public decimal SuccessfulAmount { get; set; }
    public int DeclinedCount { get; set; }
    public decimal DeclinedAmount { get; set; }
    public int RefundedCount { get; set; }
    public decimal RefundedAmount { get; set; }
    public int ReversedCount { get; set; }
    public decimal ReversedAmount { get; set; }
    public int PendingCount { get; set; }
    public int SettledCount { get; set; }
    public decimal SettledAmount { get; set; }

    // Hesaplanan alanlar
    public decimal SuccessRate => TotalCount > 0
        ? Math.Round((decimal)SuccessfulCount / TotalCount * 100, 2)
        : 0;

    public decimal AverageAmount => SuccessfulCount > 0
        ? Math.Round(SuccessfulAmount / SuccessfulCount, 2)
        : 0;
}

/// <summary>
/// İşlem tipi bazlı istatistik
/// </summary>
public class TransactionTypeStatsReadDto
{
    public int TransactionTypeId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Günlük trend verisi
/// </summary>
public class DailyTrendReadDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public int SuccessfulCount { get; set; }
    public int DeclinedCount { get; set; }
}

/// <summary>
/// Saatlik dağılım
/// </summary>
public class HourlyDistributionReadDto
{
    public int Hour { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Top merchant özeti
/// </summary>
public class TopMerchantReadDto
{
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SuccessfulAmount { get; set; }
    public decimal SuccessRate { get; set; }
}

/// <summary>
/// Decline reason istatistiği
/// </summary>
public class DeclineReasonStatsReadDto
{
    public int DeclineReasonId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Settlement batch özeti
/// </summary>
public class SettlementBatchReadDto
{
    public string BatchNumber { get; set; } = null!;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime FirstTransaction { get; set; }
    public DateTime LastTransaction { get; set; }
    public DateTime? SettledAt { get; set; }
}

/// <summary>
/// Fraud istatistiği
/// </summary>
public class FraudStatsReadDto
{
    public int FraudCheckResultId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public decimal AvgFraudScore { get; set; }
    public int MaxFraudScore { get; set; }
}