using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Enums;

/// <summary>
/// Kampanya tipleri
/// </summary>
public class CampaignType : Enumeration
{
    public static readonly CampaignType Discount = new(1, nameof(Discount), "İndirim");
    public static readonly CampaignType Cashback = new(2, nameof(Cashback), "Para İadesi");
    public static readonly CampaignType Points = new(3, nameof(Points), "Puan Kazanım");
    public static readonly CampaignType Installment = new(4, nameof(Installment), "Taksit");
    public static readonly CampaignType BonusPoints = new(5, nameof(BonusPoints), "Bonus Puan");
    public static readonly CampaignType FreeShipping = new(6, nameof(FreeShipping), "Ücretsiz Kargo");

    private CampaignType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Yüzde bazlı mı?
    /// </summary>
    public bool IsPercentageBased => this == Discount || this == Cashback;

    /// <summary>
    /// Puan bazlı mı?
    /// </summary>
    public bool IsPointsBased => this == Points || this == BonusPoints;
}