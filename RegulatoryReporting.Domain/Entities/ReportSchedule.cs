using CardMerchantSystem.Shared.Kernel;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Domain.Entities;

/// <summary>
/// Rapor zamanlama
/// </summary>
public class ReportSchedule : AggregateRoot
{
    public Guid ReportDefinitionId { get; private set; }
    public ReportDefinition Definition { get; private set; } = null!;

    // Zamanlama
    public int DayOfMonth { get; private set; } // Ayın günü (1-31)
    public int? DayOfWeek { get; private set; } // Haftanın günü (0-6, Pazar=0)
    public TimeSpan ExecutionTime { get; private set; } // Çalışma saati

    // Dönem
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    // Durum
    public bool IsEnabled { get; private set; }
    public DateTime? LastRunAt { get; private set; }
    public DateTime? NextRunAt { get; private set; }

    // Hata takibi
    public int ConsecutiveFailures { get; private set; }
    public string? LastErrorMessage { get; private set; }

    private ReportSchedule() { }

    public static Result<ReportSchedule> Create(
        Guid reportDefinitionId,
        int dayOfMonth,
        TimeSpan executionTime,
        DateTime periodStart,
        DateTime periodEnd)
    {
        if (dayOfMonth < 1 || dayOfMonth > 31)
            return Result.Failure<ReportSchedule>("Gün 1-31 arasında olmalı");

        var schedule = new ReportSchedule
        {
            ReportDefinitionId = reportDefinitionId,
            DayOfMonth = dayOfMonth,
            ExecutionTime = executionTime,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            IsEnabled = true,
            ConsecutiveFailures = 0
        };

        schedule.CalculateNextRun();
        return schedule;
    }

    /// <summary>
    /// Haftalık zamanlama için gün ayarlar
    /// </summary>
    public void SetWeeklySchedule(int dayOfWeek)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            return;

        DayOfWeek = dayOfWeek;
        CalculateNextRun();
    }

    /// <summary>
    /// Başarılı çalışma
    /// </summary>
    public void MarkSuccess()
    {
        LastRunAt = DateTime.UtcNow;
        ConsecutiveFailures = 0;
        LastErrorMessage = null;
        CalculateNextRun();
    }

    /// <summary>
    /// Başarısız çalışma
    /// </summary>
    public void MarkFailure(string errorMessage)
    {
        LastRunAt = DateTime.UtcNow;
        ConsecutiveFailures++;
        LastErrorMessage = errorMessage;

        // 5 ardışık hatada devre dışı bırak
        if (ConsecutiveFailures >= 5)
            IsEnabled = false;
        else
            CalculateNextRun();
    }

    /// <summary>
    /// Aktif et
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
        ConsecutiveFailures = 0;
        CalculateNextRun();
    }

    /// <summary>
    /// Devre dışı bırak
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
        NextRunAt = null;
    }

    private void CalculateNextRun()
    {
        if (!IsEnabled)
        {
            NextRunAt = null;
            return;
        }

        var now = DateTime.UtcNow;
        var nextRun = new DateTime(now.Year, now.Month, Math.Min(DayOfMonth, DateTime.DaysInMonth(now.Year, now.Month)));
        nextRun = nextRun.Add(ExecutionTime);

        if (nextRun <= now)
            nextRun = nextRun.AddMonths(1);

        NextRunAt = nextRun;
    }
}