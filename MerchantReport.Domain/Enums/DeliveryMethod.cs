using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Enums;

/// <summary>
/// Rapor Dağıtım Yöntemleri
/// </summary>
public class DeliveryMethod : Enumeration
{
    public static readonly DeliveryMethod None = new(1, nameof(None), "Dağıtım Yok");
    public static readonly DeliveryMethod Email = new(2, nameof(Email), "E-posta");
    public static readonly DeliveryMethod FTP = new(3, nameof(FTP), "FTP");
    public static readonly DeliveryMethod SFTP = new(4, nameof(SFTP), "SFTP");
    public static readonly DeliveryMethod API = new(5, nameof(API), "API Callback");

    private DeliveryMethod(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}