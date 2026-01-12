using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Enums;

/// <summary>
/// Tahakkuk Periyotları
/// </summary>
public class AccrualPeriod : Enumeration
{
    public static readonly AccrualPeriod Daily = new(1, nameof(Daily), "Günlük");
    public static readonly AccrualPeriod Weekly = new(2, nameof(Weekly), "Haftalık");
    public static readonly AccrualPeriod Monthly = new(3, nameof(Monthly), "Aylık");
    public static readonly AccrualPeriod Quarterly = new(4, nameof(Quarterly), "Üç Aylık");
    public static readonly AccrualPeriod Yearly = new(5, nameof(Yearly), "Yıllık");
    public static readonly AccrualPeriod OneTime = new(6, nameof(OneTime), "Tek Seferlik");

    private AccrualPeriod(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Sonraki tahakkuk tarihini hesaplar
    /// </summary>
    public DateTime GetNextAccrualDate(DateTime currentDate)
    {
        return this.Name switch
        {
            nameof(Daily) => currentDate.AddDays(1),
            nameof(Weekly) => currentDate.AddDays(7),
            nameof(Monthly) => currentDate.AddMonths(1),
            nameof(Quarterly) => currentDate.AddMonths(3),
            nameof(Yearly) => currentDate.AddYears(1),
            nameof(OneTime) => DateTime.MaxValue,
            _ => currentDate.AddMonths(1)
        };
    }
}