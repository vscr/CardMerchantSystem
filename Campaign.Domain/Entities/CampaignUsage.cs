using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Entities;

/// <summary>
/// Kampanya kullanım kaydı
/// </summary>
public class CampaignUsage : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid TransactionId { get; private set; }
    public string CardNumberMasked { get; private set; } = null!;
    public Guid MerchantId { get; private set; }
    public string MerchantCode { get; private set; } = null!;
    public decimal OriginalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public int PointsEarned { get; private set; }
    public DateTime UsedAt { get; private set; }

    // EF Core için
    private CampaignUsage() { }

    public static CampaignUsage Create(
        Guid campaignId,
        Guid transactionId,
        string cardNumberMasked,
        Guid merchantId,
        string merchantCode,
        decimal originalAmount,
        decimal discountAmount,
        decimal finalAmount,
        int pointsEarned = 0)
    {
        return new CampaignUsage
        {
            CampaignId = campaignId,
            TransactionId = transactionId,
            CardNumberMasked = cardNumberMasked,
            MerchantId = merchantId,
            MerchantCode = merchantCode,
            OriginalAmount = originalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            PointsEarned = pointsEarned,
            UsedAt = DateTime.UtcNow
        };
    }
}