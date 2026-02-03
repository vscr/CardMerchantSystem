namespace Transaction.Application.DTOs;

/// <summary>
/// İşlem istatistikleri DTO
/// </summary>
public class TransactionStatsDto
{
    /// <summary>
    /// Toplam işlem sayısı
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Toplam işlem tutarı
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Başarılı işlem sayısı
    /// </summary>
    public int SuccessfulCount { get; set; }

    /// <summary>
    /// Başarılı işlem tutarı
    /// </summary>
    public decimal SuccessfulAmount { get; set; }

    /// <summary>
    /// Reddedilen işlem sayısı
    /// </summary>
    public int DeclinedCount { get; set; }

    /// <summary>
    /// Reddedilen işlem tutarı
    /// </summary>
    public decimal DeclinedAmount { get; set; }

    /// <summary>
    /// İade işlem sayısı
    /// </summary>
    public int RefundedCount { get; set; }

    /// <summary>
    /// İade tutarı
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// İptal edilen işlem sayısı (Reversed)
    /// </summary>
    public int ReversedCount { get; set; }

    /// <summary>
    /// İptal edilen tutar
    /// </summary>
    public decimal ReversedAmount { get; set; }

    /// <summary>
    /// Bekleyen işlem sayısı
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// Takas edilmiş işlem sayısı
    /// </summary>
    public int SettledCount { get; set; }

    /// <summary>
    /// Takas edilmiş tutar
    /// </summary>
    public decimal SettledAmount { get; set; }

    /// <summary>
    /// Başarı oranı (%)
    /// </summary>
    public decimal SuccessRate => TotalCount > 0
        ? Math.Round((decimal)SuccessfulCount / TotalCount * 100, 2)
        : 0;

    /// <summary>
    /// Ortalama işlem tutarı
    /// </summary>
    public decimal AverageAmount => SuccessfulCount > 0
        ? Math.Round(SuccessfulAmount / SuccessfulCount, 2)
        : 0;

    /// <summary>
    /// İşlem tipi bazlı dağılım
    /// </summary>
    public List<TransactionTypeStatsDto> ByTransactionType { get; set; } = new();

    /// <summary>
    /// Günlük işlem trendi (son 7 gün)
    /// </summary>
    public List<DailyTransactionStatsDto> DailyTrend { get; set; } = new();

    /// <summary>
    /// İstatistik dönemi başlangıç
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// İstatistik dönemi bitiş
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Oluşturulma zamanı
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// İşlem tipi bazlı istatistik
/// </summary>
public class TransactionTypeStatsDto
{
    public string TransactionType { get; set; } = null!;
    public string TransactionTypeDisplayName { get; set; } = null!;
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Günlük işlem istatistiği
/// </summary>
public class DailyTransactionStatsDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public int SuccessfulCount { get; set; }
    public int DeclinedCount { get; set; }
}