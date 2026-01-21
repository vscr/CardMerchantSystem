using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Enums;

namespace Courier.Domain.Entities;

/// <summary>
/// Gönderi durum geçmişi
/// </summary>
public class ShipmentStatusHistory : Entity
{
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus Status { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime OccurredAt { get; private set; }
    public string? OperatorUsername { get; private set; }
    public string? Location { get; private set; }

    private ShipmentStatusHistory() { }

    public static ShipmentStatusHistory Create(
        Guid shipmentId,
        ShipmentStatus status,
        string description,
        string? operatorUsername,
        string? location = null)
    {
        return new ShipmentStatusHistory
        {
            ShipmentId = shipmentId,
            Status = status,
            Description = description,
            OccurredAt = DateTime.UtcNow,
            OperatorUsername = operatorUsername,
            Location = location
        };
    }
}