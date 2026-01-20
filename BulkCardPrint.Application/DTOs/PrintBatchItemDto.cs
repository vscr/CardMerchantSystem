namespace BulkCardPrint.Application.DTOs;

public class PrintBatchItemDto
{
    public Guid Id { get; set; }
    public Guid PrintBatchId { get; set; }
    public Guid CardApplicationId { get; set; }
    public string CustomerFullName { get; set; } = null!;
    public string CustomerTckn { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string? CardNumberMasked { get; set; }
    public string? ExpiryDate { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime? PrintedAt { get; set; }
    public DateTime? QualityCheckedAt { get; set; }
    public string? FailureReason { get; set; }
    public int SequenceNumber { get; set; }
}