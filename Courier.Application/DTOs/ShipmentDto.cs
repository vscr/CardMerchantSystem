namespace Courier.Application.DTOs;

public class ShipmentDto
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = null!;
    public string TrackingNumber { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public Guid CourierCompanyId { get; set; }
    public string CourierCompanyName { get; set; } = null!;
    public string ShipmentType { get; set; } = null!;
    public string ShipmentTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public Guid? CardApplicationId { get; set; }
    public Guid? PrintBatchItemId { get; set; }
    public string RecipientName { get; set; } = null!;
    public string RecipientPhone { get; set; } = null!;
    public string RecipientEmail { get; set; } = null!;
    public string DeliveryAddress { get; set; } = null!;
    public string DeliveryDistrict { get; set; } = null!;
    public string DeliveryCity { get; set; } = null!;
    public string DeliveryPostalCode { get; set; } = null!;
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string? DeliveredToName { get; set; }
    public bool SmsNotificationSent { get; set; }
    public bool EmailNotificationSent { get; set; }
    public decimal ShippingCost { get; set; }
    public string? Notes { get; set; }
    public string? TrackingUrl { get; set; }
    public int DeliveryAttemptCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShipmentWithDetailsDto : ShipmentDto
{
    public List<ShipmentStatusHistoryDto> StatusHistory { get; set; } = new();
    public List<DeliveryAttemptDto> DeliveryAttempts { get; set; } = new();
}

public class CreateShipmentDto
{
    public Guid CourierCompanyId { get; set; }
    public int ShipmentTypeId { get; set; }
    public string RecipientName { get; set; } = null!;
    public string RecipientPhone { get; set; } = null!;
    public string RecipientEmail { get; set; } = null!;
    public string RecipientTckn { get; set; } = null!;
    public string DeliveryAddress { get; set; } = null!;
    public string DeliveryDistrict { get; set; } = null!;
    public string DeliveryCity { get; set; } = null!;
    public string DeliveryPostalCode { get; set; } = null!;
    public Guid? CardApplicationId { get; set; }
    public Guid? PrintBatchItemId { get; set; }
}

public class UpdateDeliveryAddressDto
{
    public string DeliveryAddress { get; set; } = null!;
    public string DeliveryDistrict { get; set; } = null!;
    public string DeliveryCity { get; set; } = null!;
    public string DeliveryPostalCode { get; set; } = null!;
}

public class DeliverShipmentDto
{
    public string DeliveredToName { get; set; } = null!;
    public string DeliveredToTckn { get; set; } = null!;
    public string? SignatureData { get; set; }
}

public class FailDeliveryDto
{
    public int FailureReasonId { get; set; }
    public string? Notes { get; set; }
}