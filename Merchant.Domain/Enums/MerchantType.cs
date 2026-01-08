using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Enums;

/// <summary>
/// Üye işyeri tipleri
/// </summary>
public class MerchantType : Enumeration
{
    public static readonly MerchantType Individual = new(1, nameof(Individual), "Bireysel");
    public static readonly MerchantType SoleProprietorship = new(2, nameof(SoleProprietorship), "Şahıs Şirketi");
    public static readonly MerchantType LimitedCompany = new(3, nameof(LimitedCompany), "Limited Şirket");
    public static readonly MerchantType JointStockCompany = new(4, nameof(JointStockCompany), "Anonim Şirket");
    public static readonly MerchantType PublicInstitution = new(5, nameof(PublicInstitution), "Kamu Kurumu");

    private MerchantType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Kurumsal mı?
    /// </summary>
    public bool IsCorporate => this == LimitedCompany || this == JointStockCompany || this == PublicInstitution;
}