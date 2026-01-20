using CardMerchantSystem.Shared.Kernel;

namespace BulkCardPrint.Domain.Enums;

/// <summary>
/// Basım batch durumları
/// </summary>
public class PrintBatchStatus : Enumeration
{
    public static readonly PrintBatchStatus Created = new(1, "Created", "Oluşturuldu");
    public static readonly PrintBatchStatus FileGenerated = new(2, "FileGenerated", "Dosya Oluşturuldu");
    public static readonly PrintBatchStatus SentToVendor = new(3, "SentToVendor", "Firmaya Gönderildi");
    public static readonly PrintBatchStatus InProduction = new(4, "InProduction", "Üretimde");
    public static readonly PrintBatchStatus Completed = new(5, "Completed", "Tamamlandı");
    public static readonly PrintBatchStatus PartiallyCompleted = new(6, "PartiallyCompleted", "Kısmen Tamamlandı");
    public static readonly PrintBatchStatus Failed = new(7, "Failed", "Başarısız");
    public static readonly PrintBatchStatus Cancelled = new(8, "Cancelled", "İptal Edildi");

    private PrintBatchStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanGenerateFile => this == Created;
    public bool CanSendToVendor => this == FileGenerated;
    public bool CanStartProduction => this == SentToVendor;
    public bool CanComplete => this == InProduction;
    public bool CanCancel => this == Created || this == FileGenerated;
    public bool IsFinal => this == Completed || this == PartiallyCompleted || this == Failed || this == Cancelled;
}