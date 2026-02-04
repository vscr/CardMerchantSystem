using Transaction.Domain.Enums;

namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Dapper okuma işlemleri için internal DTO
/// Controller'da Application DTO'larına map edilir
/// </summary>
public class TransactionReadModel
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

    // Smart Enum properties (hesaplanır)
    public string TransactionType => TransactionTypeEnum?.Name ?? "Unknown";
    public string TransactionTypeDisplayName => TransactionTypeEnum?.DisplayName ?? "Bilinmiyor";
    public string Status => StatusEnum?.Name ?? "Unknown";
    public string StatusDisplayName => StatusEnum?.DisplayName ?? "Bilinmiyor";
    public string? DeclineReason => DeclineReasonEnum?.DisplayName;
    public string? FraudCheckResult => FraudCheckResultEnum?.DisplayName;

    // Helper properties
    private TransactionType? TransactionTypeEnum =>
        Transaction.Domain.Enums.TransactionType.FromId<TransactionType>(TransactionTypeId);

    private TransactionStatus? StatusEnum =>
        Transaction.Domain.Enums.TransactionStatus.FromId<TransactionStatus>(StatusId);

    private DeclineReason? DeclineReasonEnum =>
        DeclineReasonId.HasValue
            ? Transaction.Domain.Enums.DeclineReason.FromId<DeclineReason>(DeclineReasonId.Value)
            : null;

    private FraudCheckResult? FraudCheckResultEnum =>
        FraudCheckResultId.HasValue
            ? Transaction.Domain.Enums.FraudCheckResult.FromId<FraudCheckResult>(FraudCheckResultId.Value)
            : null;
}

/// <summary>
/// İşlem istatistikleri - DB'den direkt okunan model
/// </summary>
public class TransactionStatsReadModel
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
/// İşlem tipi bazlı istatistik - DB'den okunan model
/// </summary>
public class TransactionTypeStatsReadModel
{
    public int TransactionTypeId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }

    // Hesaplanan alanlar
    public string TransactionType => TransactionTypeEnum?.Name ?? "Unknown";
    public string TransactionTypeDisplayName => TransactionTypeEnum?.DisplayName ?? "Bilinmiyor";

    private TransactionType? TransactionTypeEnum =>
        Transaction.Domain.Enums.TransactionType.FromId<TransactionType>(TransactionTypeId);
}

/// <summary>
/// Günlük trend verisi
/// </summary>
public class DailyTrendReadModel
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
public class HourlyDistributionReadModel
{
    public int Hour { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Top merchant özeti
/// </summary>
public class TopMerchantReadModel
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
public class DeclineReasonStatsReadModel
{
    public int DeclineReasonId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }

    public string DeclineReason => DeclineReasonEnum?.Name ?? "Unknown";
    public string DeclineReasonDisplayName => DeclineReasonEnum?.DisplayName ?? "Bilinmiyor";

    private DeclineReason? DeclineReasonEnum =>
        Transaction.Domain.Enums.DeclineReason.FromId<DeclineReason>(DeclineReasonId);
}

/// <summary>
/// Settlement batch özeti
/// </summary>
public class SettlementBatchReadModel
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
public class FraudStatsReadModel
{
    public int FraudCheckResultId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public decimal AvgFraudScore { get; set; }
    public int MaxFraudScore { get; set; }

    public string FraudCheckResult => FraudCheckResultEnum?.Name ?? "Unknown";
    public string FraudCheckResultDisplayName => FraudCheckResultEnum?.DisplayName ?? "Bilinmiyor";

    private FraudCheckResult? FraudCheckResultEnum =>
        Transaction.Domain.Enums.FraudCheckResult.FromId<FraudCheckResult>(FraudCheckResultId);
}