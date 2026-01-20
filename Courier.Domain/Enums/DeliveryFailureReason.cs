using CardMerchantSystem.Shared.Kernel;

namespace Courier.Domain.Enums;

/// <summary>
/// Teslimat başarısızlık nedenleri
/// </summary>
public class DeliveryFailureReason : Enumeration
{
    public static readonly DeliveryFailureReason RecipientNotAvailable = new(1, "RecipientNotAvailable", "Alıcı Bulunamadı");
    public static readonly DeliveryFailureReason WrongAddress = new(2, "WrongAddress", "Adres Yanlış");
    public static readonly DeliveryFailureReason AddressNotFound = new(3, "AddressNotFound", "Adres Bulunamadı");
    public static readonly DeliveryFailureReason RefusedByRecipient = new(4, "RefusedByRecipient", "Alıcı Reddetti");
    public static readonly DeliveryFailureReason BusinessClosed = new(5, "BusinessClosed", "İşyeri Kapalı");
    public static readonly DeliveryFailureReason AccessDenied = new(6, "AccessDenied", "Erişim Engeli");
    public static readonly DeliveryFailureReason PackageDamaged = new(7, "PackageDamaged", "Paket Hasarlı");
    public static readonly DeliveryFailureReason IdVerificationFailed = new(8, "IdVerificationFailed", "Kimlik Doğrulaması Başarısız");
    public static readonly DeliveryFailureReason Other = new(9, "Other", "Diğer");

    private DeliveryFailureReason(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool RequiresAddressUpdate => this == WrongAddress || this == AddressNotFound;
    public bool CanRetryDelivery => this == RecipientNotAvailable || this == BusinessClosed || this == AccessDenied;
}