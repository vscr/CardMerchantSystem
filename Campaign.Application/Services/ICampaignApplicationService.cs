using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Application.Services;

/// <summary>
/// Kampanya uygulama servisi interface.
/// Transaction tamamlandıktan sonra kampanya kazanımlarını hesaplar.
/// </summary>
public interface ICampaignApplicationService
{
    /// <summary>
    /// İşlem için uygun kampanyaları bulur ve uygular.
    /// Post-transaction: İşlem tamamlandıktan sonra puan/cashback hesaplar.
    /// </summary>
    Task<Result<CampaignApplicationResult>> ApplyEligibleCampaignsAsync(
        CampaignApplicationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir kampanyayı işleme uygular.
    /// </summary>
    Task<Result<CampaignBenefitDto>> ApplyCampaignAsync(
        Guid campaignId,
        CampaignApplicationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kart için uygun kampanyaları listeler (preview).
    /// </summary>
    Task<IReadOnlyList<CampaignPreviewDto>> GetEligibleCampaignsAsync(
        string cardNumberMasked,
        Guid merchantId,
        decimal transactionAmount,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Kampanya uygulama isteği
/// </summary>
public record CampaignApplicationRequest
{
    public Guid TransactionId { get; init; }
    public string ReferenceNumber { get; init; } = null!;
    public string CardNumberMasked { get; init; } = null!;
    public string? CardBin { get; init; }
    public Guid MerchantId { get; init; }
    public string MerchantCode { get; init; } = null!;
    public string? MCC { get; init; }
    public decimal TransactionAmount { get; init; }
    public string Currency { get; init; } = "TRY";
}

/// <summary>
/// Kampanya uygulama sonucu
/// </summary>
public record CampaignApplicationResult
{
    public Guid TransactionId { get; init; }
    public List<CampaignBenefitDto> AppliedBenefits { get; init; } = new();
    public decimal TotalDiscount { get; init; }
    public int TotalPointsEarned { get; init; }
    public decimal TotalCashback { get; init; }
    public int CampaignsApplied => AppliedBenefits.Count;
}

/// <summary>
/// Kampanya kazanım detayı
/// </summary>
public record CampaignBenefitDto
{
    public Guid CampaignId { get; init; }
    public string CampaignCode { get; init; } = null!;
    public string CampaignName { get; init; } = null!;
    public string BenefitType { get; init; } = null!; // Discount, Points, Cashback
    public decimal DiscountAmount { get; init; }
    public int PointsEarned { get; init; }
    public decimal CashbackAmount { get; init; }
    public Guid UsageId { get; init; }
}

/// <summary>
/// Kampanya önizleme (işlem öncesi)
/// </summary>
public record CampaignPreviewDto
{
    public Guid CampaignId { get; init; }
    public string CampaignCode { get; init; } = null!;
    public string CampaignName { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string BenefitType { get; init; } = null!;
    public decimal EstimatedDiscount { get; init; }
    public int EstimatedPoints { get; init; }
    public decimal EstimatedCashback { get; init; }
    public DateTime EndDate { get; init; }
}