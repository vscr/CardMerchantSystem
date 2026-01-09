namespace Campaign.Application.DTOs;

/// <summary>
/// Kampanya oluşturma request DTO
/// </summary>
public class CreateCampaignDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CampaignTypeId { get; set; }
    public int DiscountTypeId { get; set; }
    public int TargetAudienceId { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinTransactionAmount { get; set; }
    public decimal? TotalBudget { get; set; }
    public int? MaxUsageCount { get; set; }
    public int? MaxUsagePerCustomer { get; set; }
    public int PointsMultiplier { get; set; } = 1;
}