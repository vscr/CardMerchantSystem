namespace Courier.Application.DTOs;

public class ShipmentStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime OccurredAt { get; set; }
    public string? OperatorUsername { get; set; }
    public string? Location { get; set; }
}