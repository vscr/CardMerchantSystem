namespace Courier.Application.DTOs;

public class DeliveryAttemptDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime AttemptedAt { get; set; }
    public string FailureReason { get; set; } = null!;
    public string FailureReasonDisplayName { get; set; } = null!;
    public string? Notes { get; set; }
    public string? CourierName { get; set; }
    public string? CourierPhone { get; set; }
}