using CardMerchantSystem.Shared.Kernel;

namespace BulkCardPrint.Domain.Enums;

/// <summary>
/// Basım dosya formatları
/// </summary>
public class FileFormat : Enumeration
{
    public static readonly FileFormat CSV = new(1, "CSV", "CSV Dosyası");
    public static readonly FileFormat XML = new(2, "XML", "XML Dosyası");
    public static readonly FileFormat JSON = new(3, "JSON", "JSON Dosyası");
    public static readonly FileFormat FixedWidth = new(4, "FixedWidth", "Sabit Genişlikli");
    public static readonly FileFormat ISO8583 = new(5, "ISO8583", "ISO 8583 Format");

    private FileFormat(int id, string name, string displayName)
        : base(id, name, displayName) { }

}