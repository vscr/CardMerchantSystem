using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Enums;

/// <summary>
/// Rapor Formatları
/// </summary>
public class ReportFormat : Enumeration
{
    public static readonly ReportFormat PDF = new(1, nameof(PDF), "PDF");
    public static readonly ReportFormat Excel = new(2, nameof(Excel), "Excel");
    public static readonly ReportFormat CSV = new(3, nameof(CSV), "CSV");
    public static readonly ReportFormat XML = new(4, nameof(XML), "XML");

    private ReportFormat(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public string FileExtension => this.Name switch
    {
        nameof(PDF) => ".pdf",
        nameof(Excel) => ".xlsx",
        nameof(CSV) => ".csv",
        nameof(XML) => ".xml",
        _ => ".txt"
    };

    public string ContentType => this.Name switch
    {
        nameof(PDF) => "application/pdf",
        nameof(Excel) => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        nameof(CSV) => "text/csv",
        nameof(XML) => "application/xml",
        _ => "text/plain"
    };
}