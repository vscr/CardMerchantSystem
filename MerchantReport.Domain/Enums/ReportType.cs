using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Enums;

/// <summary>
/// Rapor Tipleri
/// </summary>
public class ReportType : Enumeration
{
    public static readonly ReportType DailyStatement = new(1, nameof(DailyStatement), "Günlük Ekstre");
    public static readonly ReportType WeeklyStatement = new(2, nameof(WeeklyStatement), "Haftalık Ekstre");
    public static readonly ReportType MonthlyStatement = new(3, nameof(MonthlyStatement), "Aylık Ekstre");
    public static readonly ReportType TransactionDetail = new(4, nameof(TransactionDetail), "İşlem Detay Raporu");
    public static readonly ReportType CommissionReport = new(5, nameof(CommissionReport), "Komisyon Raporu");
    public static readonly ReportType SettlementReport = new(6, nameof(SettlementReport), "Hakediş Raporu");
    public static readonly ReportType ChargebackReport = new(7, nameof(ChargebackReport), "Chargeback Raporu");
    public static readonly ReportType RefundReport = new(8, nameof(RefundReport), "İade Raporu");
    public static readonly ReportType SummaryReport = new(9, nameof(SummaryReport), "Özet Rapor");

    private ReportType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}