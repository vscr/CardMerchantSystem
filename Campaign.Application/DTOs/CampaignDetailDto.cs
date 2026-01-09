namespace Campaign.Application.DTOs;

/// <summary>
/// Kampanya detay DTO (kurallar ve kullanımlarla birlikte)
/// </summary>
public class CampaignDetailDto
{
    public CampaignDto Campaign { get; set; } = null!;
    public List<CampaignRuleDto> Rules { get; set; } = new();
    public List<CampaignUsageDto> RecentUsages { get; set; } = new();
}

/// <summary>
/// Kampanya kuralı DTO
/// </summary>
public class CampaignRuleDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = null!;
    public string RuleType { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsActive { get; set; }
}

/// <summary>
/// Kampanya kullanım DTO
/// </summary>
public class CampaignUsageDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public string MerchantCode { get; set; } = null!;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public int PointsEarned { get; set; }
    public DateTime UsedAt { get; set; }
}