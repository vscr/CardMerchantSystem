using Campaign.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public record CalculateDiscountQuery(string CampaignCode, decimal TransactionAmount)
    : IRequest<Result<DiscountPreviewDto>>;

public class DiscountPreviewDto
{
    public string CampaignCode { get; set; } = null!;
    public string CampaignName { get; set; } = null!;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string DiscountDescription { get; set; } = null!;
}