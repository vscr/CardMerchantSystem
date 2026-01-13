using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Enums;

/// <summary>
/// Rapor Zamanlama Sıklığı
/// </summary>
public class ScheduleFrequency : Enumeration
{
    public static readonly ScheduleFrequency Daily = new(1, nameof(Daily), "Günlük");
    public static readonly ScheduleFrequency Weekly = new(2, nameof(Weekly), "Haftalık");
    public static readonly ScheduleFrequency Monthly = new(3, nameof(Monthly), "Aylık");
    public static readonly ScheduleFrequency OnDemand = new(4, nameof(OnDemand), "Talep Üzerine");

    private ScheduleFrequency(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Bir sonraki çalışma zamanını hesaplar
    /// </summary>
    public DateTime GetNextRunTime(DateTime currentTime, int dayOfWeek = 1, int dayOfMonth = 1, TimeSpan? runTime = null)
    {
        var time = runTime ?? new TimeSpan(6, 0, 0); // Varsayılan: 06:00

        return this.Name switch
        {
            nameof(Daily) => currentTime.Date.AddDays(1).Add(time),
            nameof(Weekly) => GetNextWeeklyRun(currentTime, dayOfWeek, time),
            nameof(Monthly) => GetNextMonthlyRun(currentTime, dayOfMonth, time),
            _ => currentTime
        };
    }

    private static DateTime GetNextWeeklyRun(DateTime current, int dayOfWeek, TimeSpan time)
    {
        var daysUntilTarget = ((dayOfWeek - (int)current.DayOfWeek + 7) % 7);
        if (daysUntilTarget == 0) daysUntilTarget = 7;
        return current.Date.AddDays(daysUntilTarget).Add(time);
    }

    private static DateTime GetNextMonthlyRun(DateTime current, int dayOfMonth, TimeSpan time)
    {
        var nextMonth = current.AddMonths(1);
        var day = Math.Min(dayOfMonth, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
        return new DateTime(nextMonth.Year, nextMonth.Month, day).Add(time);
    }
}