namespace Campaign.Application.DTOs;

/// <summary>
/// Kampanya response DTO
/// </summary>
public class CampaignDto
{
    public Guid Id { get; set; }
    public string CampaignCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string CampaignType { get; set; } = null!;
    public string CampaignTypeDisplayName { get; set; } = null!;
    public string DiscountType { get; set; } = null!;
    public string DiscountTypeDisplayName { get; set; } = null!;
    public string TargetAudience { get; set; } = null!;
    public string TargetAudienceDisplayName { get; set; } = null!;

    // İndirim Bilgileri
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinTransactionAmount { get; set; }
    public int PointsMultiplier { get; set; }

    // Bütçe ve Limit Bilgileri
    public decimal? TotalBudget { get; set; }
    public decimal UsedBudget { get; set; }
    public decimal? RemainingBudget { get; set; }
    public int? MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public int? RemainingUsageCount { get; set; }
    public int? MaxUsagePerCustomer { get; set; }

    // Tarih Bilgileri
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrentlyActive { get; set; }

    // Üye İşyeri Bilgileri
    public bool IsAllMerchants { get; set; }
    public List<Guid>? AllowedMerchantIds { get; set; }

    // Onay Bilgileri
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Genel Bilgiler
    public DateTime CreatedAt { get; set; }
    public int RuleCount { get; set; }
}