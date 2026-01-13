using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Entities;

/// <summary>
/// Ekstre Kesim Tarihi Ayarları
/// </summary>
public class StatementPeriodConfig : AggregateRoot
{
    public string CardNumber { get; private set; } = null!;
    public int StatementDay { get; private set; } // 1-28 arası
    public int PaymentDueDays { get; private set; } // Kesimden sonra kaç gün
    public decimal InterestRate { get; private set; }
    public decimal CashAdvanceInterestRate { get; private set; }
    public decimal MinimumPaymentRate { get; private set; }
    public decimal MinimumPaymentAmount { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core için
    private StatementPeriodConfig() { }

    /// <summary>
    /// Yeni kesim ayarı oluşturur
    /// </summary>
    public static Result<StatementPeriodConfig> Create(
        string cardNumber,
        int statementDay,
        int paymentDueDays = 10,
        decimal interestRate = 42.0m,
        decimal cashAdvanceInterestRate = 54.0m,
        decimal minimumPaymentRate = 20.0m,
        decimal minimumPaymentAmount = 100.0m)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return Result.Failure<StatementPeriodConfig>("Kart numarası boş olamaz");

        if (statementDay < 1 || statementDay > 28)
            return Result.Failure<StatementPeriodConfig>("Kesim günü 1-28 arasında olmalı");

        if (paymentDueDays < 1 || paymentDueDays > 30)
            return Result.Failure<StatementPeriodConfig>("Ödeme vadesi 1-30 gün arasında olmalı");

        var config = new StatementPeriodConfig
        {
            CardNumber = cardNumber,
            StatementDay = statementDay,
            PaymentDueDays = paymentDueDays,
            InterestRate = interestRate,
            CashAdvanceInterestRate = cashAdvanceInterestRate,
            MinimumPaymentRate = minimumPaymentRate,
            MinimumPaymentAmount = minimumPaymentAmount,
            IsActive = true
        };

        return config;
    }

    /// <summary>
    /// Dönem tarihlerini hesaplar
    /// </summary>
    public (DateTime periodStart, DateTime periodEnd, DateTime dueDate) CalculatePeriodDates(DateTime referenceDate)
    {
        // Dönem sonu: Referans ayının kesim günü
        var periodEnd = new DateTime(referenceDate.Year, referenceDate.Month, StatementDay);

        // Eğer referans tarih kesim gününden sonraysa, bir sonraki ayın kesimine bak
        if (referenceDate.Day > StatementDay)
        {
            periodEnd = periodEnd.AddMonths(1);
        }

        // Dönem başı: Bir önceki ayın kesim gününün ertesi günü
        var periodStart = periodEnd.AddMonths(-1).AddDays(1);

        // Son ödeme tarihi
        var dueDate = periodEnd.AddDays(PaymentDueDays);

        return (periodStart, periodEnd, dueDate);
    }

    public void UpdateInterestRate(decimal rate) => InterestRate = rate;
    public void UpdateStatementDay(int day)
    {
        if (day >= 1 && day <= 28)
            StatementDay = day;
    }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}