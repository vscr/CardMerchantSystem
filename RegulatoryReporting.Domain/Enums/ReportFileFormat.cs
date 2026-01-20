using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Enums;

/// <summary>
/// Rapor dosya formatları
/// </summary>
public class ReportFileFormat : Enumeration
{
    public static readonly ReportFileFormat XML = new(1, "XML", "XML Dosyası");
    public static readonly ReportFileFormat CSV = new(2, "CSV", "CSV Dosyası");
    public static readonly ReportFileFormat Excel = new(3, "Excel", "Excel Dosyası");
    public static readonly ReportFileFormat PDF = new(4, "PDF", "PDF Dosyası");
    public static readonly ReportFileFormat JSON = new(5, "JSON", "JSON Dosyası");

    private ReportFileFormat(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public string GetFileExtension()
    {
        return this.Id switch
        {
            1 => ".xml",
            2 => ".csv",
            3 => ".xlsx",
            4 => ".pdf",
            5 => ".json",
            _ => ".txt"
        };
    }
}