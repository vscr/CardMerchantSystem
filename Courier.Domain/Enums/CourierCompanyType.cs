using CardMerchantSystem.Shared.Kernel;

namespace Courier.Domain.Enums;

/// <summary>
/// Kurye firma tipleri
/// </summary>
public class CourierCompanyType : Enumeration
{
    public static readonly CourierCompanyType National = new(1, "National", "Ulusal");
    public static readonly CourierCompanyType International = new(2, "International", "Uluslararası");
    public static readonly CourierCompanyType Regional = new(3, "Regional", "Bölgesel");
    public static readonly CourierCompanyType Express = new(4, "Express", "Ekspres");

    private CourierCompanyType(int id, string name, string displayName)
        : base(id, name, displayName) { }
}