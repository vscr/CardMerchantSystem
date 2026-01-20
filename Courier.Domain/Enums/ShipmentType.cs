using CardMerchantSystem.Shared.Kernel;

namespace Courier.Domain.Enums;

/// <summary>
/// Gönderi tipleri
/// </summary>
public class ShipmentType : Enumeration
{
    public static readonly ShipmentType CardDelivery = new(1, "CardDelivery", "Kart Teslimatı");
    public static readonly ShipmentType PinMailer = new(2, "PinMailer", "PIN Zarfı");
    public static readonly ShipmentType Statement = new(3, "Statement", "Ekstre");
    public static readonly ShipmentType Document = new(4, "Document", "Belge");
    public static readonly ShipmentType CardAndPin = new(5, "CardAndPin", "Kart ve PIN");

    private ShipmentType(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool RequiresIdVerification => this == CardDelivery || this == CardAndPin;
    public bool RequiresSignature => this == CardDelivery || this == CardAndPin || this == PinMailer;
}