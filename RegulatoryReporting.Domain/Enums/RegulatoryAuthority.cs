using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Enums;

/// <summary>
/// Düzenleyici kurumlar
/// </summary>
public class RegulatoryAuthority : Enumeration
{
    public static readonly RegulatoryAuthority BDDK = new(1, "BDDK", "Bankacılık Düzenleme ve Denetleme Kurumu");
    public static readonly RegulatoryAuthority TCMB = new(2, "TCMB", "Türkiye Cumhuriyet Merkez Bankası");
    public static readonly RegulatoryAuthority SPK = new(3, "SPK", "Sermaye Piyasası Kurulu");
    public static readonly RegulatoryAuthority MASAK = new(4, "MASAK", "Mali Suçları Araştırma Kurulu");
    public static readonly RegulatoryAuthority GIB = new(5, "GIB", "Gelir İdaresi Başkanlığı");
    public static readonly RegulatoryAuthority BKM = new(6, "BKM", "Bankalararası Kart Merkezi");

    private RegulatoryAuthority(int id, string name, string displayName)
        : base(id, name, displayName) { }
}