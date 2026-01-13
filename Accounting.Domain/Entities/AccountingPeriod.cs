using Accounting.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Entities;

/// <summary>
/// Muhasebe Dönemi
/// </summary>
public class AccountingPeriod : AggregateRoot
{
    public string PeriodCode { get; private set; } = null!; // 202501, 202502
    public string PeriodName { get; private set; } = null!;
    public int Year { get; private set; }
    public int Month { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PeriodStatus Status { get; private set; } = null!;
    public DateTime? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }

    // EF Core için
    private AccountingPeriod() { }

    /// <summary>
    /// Yeni dönem oluşturur
    /// </summary>
    public static Result<AccountingPeriod> Create(int year, int month)
    {
        if (year < 2000 || year > 2100)
            return Result.Failure<AccountingPeriod>("Geçersiz yıl");

        if (month < 1 || month > 12)
            return Result.Failure<AccountingPeriod>("Geçersiz ay");

        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var period = new AccountingPeriod
        {
            PeriodCode = $"{year}{month:D2}",
            PeriodName = $"{year} - {GetMonthName(month)}",
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Status = PeriodStatus.Open
        };

        return period;
    }

    /// <summary>
    /// Dönemi kapatmaya başla
    /// </summary>
    public Result StartClosing()
    {
        if (Status != PeriodStatus.Open)
            return Result.Failure("Sadece açık dönemler kapatılabilir");

        Status = PeriodStatus.Closing;
        return Result.Success();
    }

    /// <summary>
    /// Dönemi kapat
    /// </summary>
    public Result Close(string closedBy)
    {
        if (Status != PeriodStatus.Closing)
            return Result.Failure("Dönem kapatma işlemi başlatılmamış");

        Status = PeriodStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = closedBy;
        return Result.Success();
    }

    /// <summary>
    /// Dönemi kilitle
    /// </summary>
    public Result Lock()
    {
        if (Status != PeriodStatus.Closed)
            return Result.Failure("Sadece kapalı dönemler kilitlenebilir");

        Status = PeriodStatus.Locked;
        return Result.Success();
    }

    /// <summary>
    /// Dönemi yeniden aç
    /// </summary>
    public Result Reopen()
    {
        if (Status == PeriodStatus.Locked)
            return Result.Failure("Kilitli dönemler açılamaz");

        Status = PeriodStatus.Open;
        ClosedAt = null;
        ClosedBy = null;
        return Result.Success();
    }

    private static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "Ocak",
            2 => "Şubat",
            3 => "Mart",
            4 => "Nisan",
            5 => "Mayıs",
            6 => "Haziran",
            7 => "Temmuz",
            8 => "Ağustos",
            9 => "Eylül",
            10 => "Ekim",
            11 => "Kasım",
            12 => "Aralık",
            _ => ""
        };
    }
}