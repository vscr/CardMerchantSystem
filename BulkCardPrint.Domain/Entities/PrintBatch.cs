using CardMerchantSystem.Shared.Kernel;
using BulkCardPrint.Domain.Enums;

namespace BulkCardPrint.Domain.Entities;

/// <summary>
/// Toplu kart basım batch'i
/// </summary>
public class PrintBatch : AggregateRoot
{
    public string BatchNumber { get; private set; } = null!;
    public Guid PrintVendorId { get; private set; }
    public PrintVendor Vendor { get; private set; } = null!;

    // Durum
    public PrintBatchStatus Status { get; private set; } = null!;

    // Sayaçlar
    public int TotalItemCount { get; private set; }
    public int PrintedCount { get; private set; }
    public int FailedCount { get; private set; }

    // Dosya bilgileri
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public DateTime? FileGeneratedAt { get; private set; }
    public string? FileChecksum { get; private set; }

    // Tarihler
    public DateTime? SentToVendorAt { get; private set; }
    public DateTime? ProductionStartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Notlar
    public string? Notes { get; private set; }
    public string? FailureReason { get; private set; }

    // Items
    private readonly List<PrintBatchItem> _items = new();
    public IReadOnlyCollection<PrintBatchItem> Items => _items.AsReadOnly();

    private PrintBatch() { }

    public static Result<PrintBatch> Create(Guid printVendorId)
    {
        var batch = new PrintBatch
        {
            BatchNumber = GenerateBatchNumber(),
            PrintVendorId = printVendorId,
            Status = PrintBatchStatus.Created,
            TotalItemCount = 0,
            PrintedCount = 0,
            FailedCount = 0
        };

        return batch;
    }

    /// <summary>
    /// Batch'e kart ekler
    /// </summary>
    public Result AddItem(PrintBatchItem item)
    {
        if (!Status.CanGenerateFile && Status != PrintBatchStatus.Created)
            return Result.Failure("Bu durumda item eklenemez");

        _items.Add(item);
        TotalItemCount = _items.Count;
        return Result.Success();
    }

    /// <summary>
    /// Birden fazla item ekler
    /// </summary>
    public Result AddItems(IEnumerable<PrintBatchItem> items)
    {
        if (!Status.CanGenerateFile && Status != PrintBatchStatus.Created)
            return Result.Failure("Bu durumda item eklenemez");

        _items.AddRange(items);
        TotalItemCount = _items.Count;
        return Result.Success();
    }

    /// <summary>
    /// Dosya oluşturuldu olarak işaretle
    /// </summary>
    public Result MarkFileGenerated(string fileName, string filePath, string checksum, string operatorUsername)
    {
        if (!Status.CanGenerateFile)
            return Result.Failure($"Bu durumda dosya oluşturulamaz. Mevcut durum: {Status.DisplayName}");

        if (_items.Count == 0)
            return Result.Failure("Batch'te item yok");

        Status = PrintBatchStatus.FileGenerated;
        FileName = fileName;
        FilePath = filePath;
        FileChecksum = checksum;
        FileGeneratedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Firmaya gönderildi olarak işaretle
    /// </summary>
    public Result MarkSentToVendor(string operatorUsername)
    {
        if (!Status.CanSendToVendor)
            return Result.Failure($"Bu durumda gönderilemez. Mevcut durum: {Status.DisplayName}");

        Status = PrintBatchStatus.SentToVendor;
        SentToVendorAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Üretim başladı olarak işaretle
    /// </summary>
    public Result StartProduction(string operatorUsername)
    {
        if (!Status.CanStartProduction)
            return Result.Failure($"Bu durumda üretim başlatılamaz. Mevcut durum: {Status.DisplayName}");

        Status = PrintBatchStatus.InProduction;
        ProductionStartedAt = DateTime.UtcNow;

        foreach (var item in _items.Where(i => i.Status == PrintItemStatus.Pending))
        {
            item.StartProduction();
        }

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Batch'i tamamla
    /// </summary>
    public Result Complete(string operatorUsername)
    {
        if (!Status.CanComplete)
            return Result.Failure($"Bu durumda tamamlanamaz. Mevcut durum: {Status.DisplayName}");

        PrintedCount = _items.Count(i => i.Status == PrintItemStatus.Printed || i.Status == PrintItemStatus.ReadyForDelivery);
        FailedCount = _items.Count(i => i.Status == PrintItemStatus.QualityFailed || i.Status == PrintItemStatus.Cancelled);

        if (FailedCount > 0 && PrintedCount > 0)
        {
            Status = PrintBatchStatus.PartiallyCompleted;
        }
        else if (FailedCount == TotalItemCount)
        {
            Status = PrintBatchStatus.Failed;
            FailureReason = "Tüm kartlar başarısız";
        }
        else
        {
            Status = PrintBatchStatus.Completed;
        }

        CompletedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i iptal et
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = PrintBatchStatus.Cancelled;
        FailureReason = reason;

        foreach (var item in _items.Where(i => !i.Status.IsFinal))
        {
            item.Cancel("Batch iptal edildi");
        }

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Not ekle
    /// </summary>
    public void AddNote(string note)
    {
        Notes = string.IsNullOrEmpty(Notes) ? note : $"{Notes}\n{note}";
    }

    private static string GenerateBatchNumber()
    {
        return $"PB{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}