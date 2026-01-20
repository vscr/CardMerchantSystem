namespace BulkCardPrint.Application.DTOs;

public class PrintBatchDto
{
    public Guid Id { get; set; }
    public string BatchNumber { get; set; } = null!;
    public Guid PrintVendorId { get; set; }
    public string VendorName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public int TotalItemCount { get; set; }
    public int PrintedCount { get; set; }
    public int FailedCount { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public DateTime? FileGeneratedAt { get; set; }
    public DateTime? SentToVendorAt { get; set; }
    public DateTime? ProductionStartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PrintBatchWithItemsDto : PrintBatchDto
{
    public List<PrintBatchItemDto> Items { get; set; } = new();
}

public class CreatePrintBatchDto
{
    public Guid PrintVendorId { get; set; }
    public List<Guid> CardApplicationIds { get; set; } = new();
}