using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Enums;

namespace Courier.Domain.Entities;

/// <summary>
/// Teslimat denemesi
/// </summary>
public class DeliveryAttempt : Entity
{
    public Guid ShipmentId { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public DeliveryFailureReason FailureReason { get; private set; } = null!;
    public string? Notes { get; private set; }
    public string? CourierName { get; private set; }
    public string? CourierPhone { get; private set; }

    private DeliveryAttempt() { }

    public static DeliveryAttempt Create(
        Guid shipmentId,
        DeliveryFailureReason failureReason,
        string? notes,
        string? courierName = null,
        string? courierPhone = null)
    {
        return new DeliveryAttempt
        {
            ShipmentId = shipmentId,
            AttemptedAt = DateTime.UtcNow,
            FailureReason = failureReason,
            Notes = notes,
            CourierName = courierName,
            CourierPhone = courierPhone
        };
    }

    public void SetAttemptNumber(int number)
    {
        AttemptNumber = number;
    }
}