using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Enums;

namespace Courier.Domain.Entities;

/// <summary>
/// Gönderi
/// </summary>
public class Shipment : AggregateRoot
{
    public string ShipmentNumber { get; private set; } = null!;
    public string TrackingNumber { get; private set; } = null!;
    public string Barcode { get; private set; } = null!;

    // Kurye firması
    public Guid CourierCompanyId { get; private set; }
    public CourierCompany Company { get; private set; } = null!;

    // Gönderi tipi
    public ShipmentType ShipmentType { get; private set; } = null!;

    // Durum
    public ShipmentStatus Status { get; private set; } = null!;

    // İlişkili kayıtlar
    public Guid? CardApplicationId { get; private set; }
    public Guid? PrintBatchItemId { get; private set; }

    // Alıcı bilgileri
    public string RecipientName { get; private set; } = null!;
    public string RecipientPhone { get; private set; } = null!;
    public string RecipientEmail { get; private set; } = null!;
    public string RecipientTckn { get; private set; } = null!;

    // Teslimat adresi
    public string DeliveryAddress { get; private set; } = null!;
    public string DeliveryDistrict { get; private set; } = null!;
    public string DeliveryCity { get; private set; } = null!;
    public string DeliveryPostalCode { get; private set; } = null!;

    // Tarihler
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }

    // Teslimat bilgileri
    public string? DeliveredToName { get; private set; }
    public string? DeliveredToTckn { get; private set; }
    public string? SignatureData { get; private set; }

    // Bildirim
    public bool SmsNotificationSent { get; private set; }
    public bool EmailNotificationSent { get; private set; }

    // Fiyat
    public decimal ShippingCost { get; private set; }

    // Notlar
    public string? Notes { get; private set; }

    // Durum geçmişi
    private readonly List<ShipmentStatusHistory> _statusHistory = new();
    public IReadOnlyCollection<ShipmentStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    // Teslimat denemeleri
    private readonly List<DeliveryAttempt> _deliveryAttempts = new();
    public IReadOnlyCollection<DeliveryAttempt> DeliveryAttempts => _deliveryAttempts.AsReadOnly();

    private Shipment() { }

    public static Result<Shipment> Create(
        Guid courierCompanyId,
        ShipmentType shipmentType,
        string recipientName,
        string recipientPhone,
        string recipientEmail,
        string recipientTckn,
        string deliveryAddress,
        string deliveryDistrict,
        string deliveryCity,
        string deliveryPostalCode,
        int deliveryDays,
        decimal shippingCost,
        Guid? cardApplicationId = null,
        Guid? printBatchItemId = null)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            return Result.Failure<Shipment>("Alıcı adı boş olamaz");

        if (string.IsNullOrWhiteSpace(deliveryAddress))
            return Result.Failure<Shipment>("Teslimat adresi boş olamaz");

        var shipment = new Shipment
        {
            ShipmentNumber = GenerateShipmentNumber(),
            TrackingNumber = GenerateTrackingNumber(),
            Barcode = GenerateBarcode(),
            CourierCompanyId = courierCompanyId,
            ShipmentType = shipmentType,
            Status = ShipmentStatus.Created,
            CardApplicationId = cardApplicationId,
            PrintBatchItemId = printBatchItemId,
            RecipientName = recipientName,
            RecipientPhone = recipientPhone,
            RecipientEmail = recipientEmail,
            RecipientTckn = recipientTckn,
            DeliveryAddress = deliveryAddress,
            DeliveryDistrict = deliveryDistrict,
            DeliveryCity = deliveryCity,
            DeliveryPostalCode = deliveryPostalCode,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(deliveryDays),
            ShippingCost = shippingCost,
            SmsNotificationSent = false,
            EmailNotificationSent = false
        };

        shipment.AddStatusHistory("Gönderi oluşturuldu", null);

        return shipment;
    }

    /// <summary>
    /// Kurye tarafından alındı
    /// </summary>
    public Result MarkAsPickedUp(string operatorUsername)
    {
        if (Status != ShipmentStatus.Created)
            return Result.Failure($"Bu durumda alınamaz. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
        AddStatusHistory("Kurye tarafından alındı", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Yolda
    /// </summary>
    public Result MarkAsInTransit(string location, string operatorUsername)
    {
        if (!Status.IsInProgress && Status != ShipmentStatus.PickedUp)
            return Result.Failure($"Bu durumda transit yapılamaz. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.InTransit;
        AddStatusHistory($"Transfer merkezi: {location}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Dağıtımda
    /// </summary>
    public Result MarkAsOutForDelivery(string operatorUsername)
    {
        if (Status != ShipmentStatus.InTransit && Status != ShipmentStatus.PickedUp)
            return Result.Failure($"Bu durumda dağıtıma çıkarılamaz. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.OutForDelivery;
        AddStatusHistory("Dağıtıma çıktı", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Teslim edildi
    /// </summary>
    public Result MarkAsDelivered(
        string deliveredToName,
        string deliveredToTckn,
        string? signatureData,
        string operatorUsername)
    {
        if (Status != ShipmentStatus.OutForDelivery)
            return Result.Failure($"Bu durumda teslim edilemez. Mevcut durum: {Status.DisplayName}");

        if (ShipmentType.RequiresIdVerification && string.IsNullOrWhiteSpace(deliveredToTckn))
            return Result.Failure("Bu gönderi tipi için kimlik doğrulaması gerekli");

        Status = ShipmentStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        DeliveredToName = deliveredToName;
        DeliveredToTckn = deliveredToTckn;
        SignatureData = signatureData;
        AddStatusHistory($"Teslim edildi: {deliveredToName}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Teslimat başarısız
    /// </summary>
    public Result MarkAsDeliveryFailed(DeliveryFailureReason reason, string? notes, string operatorUsername)
    {
        if (Status != ShipmentStatus.OutForDelivery)
            return Result.Failure($"Bu durumda başarısız yapılamaz. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.DeliveryFailed;

        var attempt = DeliveryAttempt.Create(Id, reason, notes);
        _deliveryAttempts.Add(attempt);

        AddStatusHistory($"Teslimat başarısız: {reason.DisplayName}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Tekrar deneme için hazırla
    /// </summary>
    public Result PrepareForRetry(string? newAddress, string operatorUsername)
    {
        if (!Status.CanRetry)
            return Result.Failure($"Bu durumda tekrar denenemez. Mevcut durum: {Status.DisplayName}");

        if (!string.IsNullOrWhiteSpace(newAddress))
            DeliveryAddress = newAddress;

        Status = ShipmentStatus.InTransit;
        ExpectedDeliveryDate = DateTime.UtcNow.AddDays(2);
        AddStatusHistory("Tekrar teslimat için hazırlandı", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// İade edildi
    /// </summary>
    public Result MarkAsReturned(string reason, string operatorUsername)
    {
        if (Status != ShipmentStatus.DeliveryFailed)
            return Result.Failure($"Bu durumda iade edilemez. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.Returned;
        AddStatusHistory($"İade edildi: {reason}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// İptal et
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.Cancelled;
        AddStatusHistory($"İptal edildi: {reason}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Kayıp olarak işaretle
    /// </summary>
    public Result MarkAsLost(string operatorUsername)
    {
        if (Status.IsFinal)
            return Result.Failure($"Bu durumda kayıp yapılamaz. Mevcut durum: {Status.DisplayName}");

        Status = ShipmentStatus.Lost;
        AddStatusHistory("Gönderi kayıp olarak işaretlendi", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// SMS bildirimi gönderildi
    /// </summary>
    public void MarkSmsNotificationSent()
    {
        SmsNotificationSent = true;
    }

    /// <summary>
    /// Email bildirimi gönderildi
    /// </summary>
    public void MarkEmailNotificationSent()
    {
        EmailNotificationSent = true;
    }

    /// <summary>
    /// Not ekle
    /// </summary>
    public void AddNote(string note)
    {
        Notes = string.IsNullOrEmpty(Notes) ? note : $"{Notes}\n{note}";
    }

    /// <summary>
    /// Adres güncelle
    /// </summary>
    public Result UpdateDeliveryAddress(
        string address,
        string district,
        string city,
        string postalCode,
        string operatorUsername)
    {
        if (Status.IsFinal || Status == ShipmentStatus.OutForDelivery)
            return Result.Failure("Adres güncellenemez");

        DeliveryAddress = address;
        DeliveryDistrict = district;
        DeliveryCity = city;
        DeliveryPostalCode = postalCode;
        AddStatusHistory("Teslimat adresi güncellendi", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    private void AddStatusHistory(string description, string? operatorUsername)
    {
        var history = ShipmentStatusHistory.Create(Id, Status, description, operatorUsername);
        _statusHistory.Add(history);
    }

    private static string GenerateShipmentNumber()
    {
        return $"SHP{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private static string GenerateTrackingNumber()
    {
        return $"TRK{Random.Shared.Next(100000000, 999999999)}";
    }

    private static string GenerateBarcode()
    {
        return $"{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(10000000, 99999999)}";
    }
}