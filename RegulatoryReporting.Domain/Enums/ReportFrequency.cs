using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Enums;

/// <summary>
/// Rapor periyotları
/// </summary>
public class ReportFrequency : Enumeration
{
    public static readonly ReportFrequency Daily = new(1, "Daily", "Günlük");
    public static readonly ReportFrequency Weekly = new(2, "Weekly", "Haftalık");
    public static readonly ReportFrequency Monthly = new(3, "Monthly", "Aylık");
    public static readonly ReportFrequency Quarterly = new(4, "Quarterly", "Üç Aylık");
    public static readonly ReportFrequency Yearly = new(5, "Yearly", "Yıllık");
    public static readonly ReportFrequency OnDemand = new(6, "OnDemand", "Talep Üzerine");

    private ReportFrequency(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public int GetDaysInterval()
    {
        return this.Id switch
        {
            1 => 1,
            2 => 7,
            3 => 30,
            4 => 90,
            5 => 365,
            _ => 0
        };
    }
}