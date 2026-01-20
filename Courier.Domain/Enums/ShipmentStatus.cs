using CardMerchantSystem.Shared.Kernel;

namespace Courier.Domain.Enums;

/// <summary>
/// Gönderi durumları
/// </summary>
public class ShipmentStatus : Enumeration
{
    public static readonly ShipmentStatus Created = new(1, "Created", "Oluşturuldu");
    public static readonly ShipmentStatus PickedUp = new(2, "PickedUp", "Alındı");
    public static readonly ShipmentStatus InTransit = new(3, "InTransit", "Yolda");
    public static readonly ShipmentStatus OutForDelivery = new(4, "OutForDelivery", "Dağıtımda");
    public static readonly ShipmentStatus Delivered = new(5, "Delivered", "Teslim Edildi");
    public static readonly ShipmentStatus DeliveryFailed = new(6, "DeliveryFailed", "Teslim Edilemedi");
    public static readonly ShipmentStatus Returned = new(7, "Returned", "İade Edildi");
    public static readonly ShipmentStatus Cancelled = new(8, "Cancelled", "İptal Edildi");
    public static readonly ShipmentStatus Lost = new(9, "Lost", "Kayıp");

    private ShipmentStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool CanCancel => this == Created || this == PickedUp;
    public bool CanRetry => this == DeliveryFailed;
    public bool IsFinal => this == Delivered || this == Returned || this == Cancelled || this == Lost;
    public bool IsInProgress => this == PickedUp || this == InTransit || this == OutForDelivery;
}