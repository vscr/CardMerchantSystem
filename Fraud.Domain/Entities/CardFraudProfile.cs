using CardMerchantSystem.Shared.Kernel;

namespace Fraud.Domain.Entities;

/// <summary>
/// Kart bazlı fraud profili. PayGuard'daki CardStatistics karşılığı.
/// Her kart için istatistikler tutulur ve periodic rule'larda kullanılır.
/// Real-time güncellenir.
/// </summary>
public class CardFraudProfile : Entity
{
    public string MaskedCardNo { get; private set; } = null!;

    // ── İşlem İstatistikleri ──
    public int TotalTransactionCount { get; private set; }
    public int Last1HourTxCount { get; private set; }
    public int Last24HourTxCount { get; private set; }
    public int Last7DayTxCount { get; private set; }

    // ── Tutar İstatistikleri ──
    public decimal TotalTransactionAmount { get; private set; }
    public decimal Last1HourTxAmount { get; private set; }
    public decimal Last24HourTxAmount { get; private set; }

    // ── Konum İstatistikleri ──
    public int Last24HourDistinctCountryCount { get; private set; }
    public int Last24HourDistinctMerchantCount { get; private set; }
    public string? LastTransactionCountry { get; private set; }
    public string? LastMerchantId { get; private set; }

    // ── Fraud İstatistikleri ──
    public int TotalHitScenarioCount { get; private set; }
    public int TotalFraudConfirmedCount { get; private set; }
    public int DeclinedTransactionCount { get; private set; }
    public DateTime? LastFraudAlertDate { get; private set; }
    public int CurrentRiskScore { get; private set; }

    // ── Zaman ──
    public DateTime? LastTransactionDate { get; private set; }
    public DateTime? FirstTransactionDate { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private CardFraudProfile() { } // EF Core

    public CardFraudProfile(string maskedCardNo)
    {
        MaskedCardNo = maskedCardNo;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>Yeni işlem geldiğinde profili güncelle</summary>
    public void RecordTransaction(decimal amount, string? countryCode, string? merchantId)
    {
        TotalTransactionCount++;
        TotalTransactionAmount += amount;
        Last1HourTxCount++;
        Last1HourTxAmount += amount;
        Last24HourTxCount++;
        Last24HourTxAmount += amount;
        Last7DayTxCount++;

        LastTransactionCountry = countryCode;
        LastMerchantId = merchantId;
        LastTransactionDate = DateTime.UtcNow;
        FirstTransactionDate ??= DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>Senaryo tetiklendiğinde</summary>
    public void RecordHitScenario(int score)
    {
        TotalHitScenarioCount++;
        LastFraudAlertDate = DateTime.UtcNow;
        CurrentRiskScore = Math.Min(100, CurrentRiskScore + score / 2);
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>Fraud onaylandığında</summary>
    public void RecordConfirmedFraud()
    {
        TotalFraudConfirmedCount++;
        CurrentRiskScore = Math.Min(100, CurrentRiskScore + 25);
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>İşlem reddedildiğinde</summary>
    public void RecordDeclinedTransaction()
    {
        DeclinedTransactionCount++;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>Periyodik reset (Hangfire job ile saatlik çalışır)</summary>
    public void ResetHourlyCounters()
    {
        Last1HourTxCount = 0;
        Last1HourTxAmount = 0;
        LastUpdated = DateTime.UtcNow;
    }

    public void ResetDailyCounters()
    {
        Last24HourTxCount = 0;
        Last24HourTxAmount = 0;
        Last24HourDistinctCountryCount = 0;
        Last24HourDistinctMerchantCount = 0;
        LastUpdated = DateTime.UtcNow;
    }

    public void ResetWeeklyCounters()
    {
        Last7DayTxCount = 0;
        LastUpdated = DateTime.UtcNow;
    }
}