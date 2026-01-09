namespace Campaign.Application.DTOs;

/// <summary>
/// Kampanya uygulama request DTO
/// </summary>
public class ApplyCampaignDto
{
    public string CampaignCode { get; set; } = null!;
    public Guid TransactionId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public decimal TransactionAmount { get; set; }
    public string? MCC { get; set; }
    public string? CardBIN { get; set; }
}

/// <summary>
/// Kampanya uygulama sonuç DTO
/// </summary>
public class ApplyCampaignResultDto
{
    public bool IsApplied { get; set; }
    public string CampaignCode { get; set; } = null!;
    public string CampaignName { get; set; } = null!;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public int PointsEarned { get; set; }
    public string? ErrorMessage { get; set; }
}