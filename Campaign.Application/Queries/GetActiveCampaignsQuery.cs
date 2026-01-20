using Campaign.Application.DTOs;
using Campaign.Domain.Entities;
using Campaign.Domain.Repositories;
using MediatR;

namespace Campaign.Application.Queries;

public record GetActiveCampaignsQuery() : IRequest<IReadOnlyList<CampaignDto>>;
public class GetActiveCampaignsQueryHandler
    : IRequestHandler<GetActiveCampaignsQuery, IReadOnlyList<CampaignDto>>
{
    private readonly ICampaignRepository _repository;

    public GetActiveCampaignsQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CampaignDto>> Handle(
        GetActiveCampaignsQuery request,
        CancellationToken cancellationToken)
    {
        var campaigns = await _repository.GetActiveAsync(cancellationToken);

        return campaigns.Select(MapToDto).ToList();
    }

    private static CampaignDto MapToDto(CampaignAggregate c)
    {
        return new CampaignDto
        {
            Id = c.Id,
            CampaignCode = c.CampaignCode,
            Name = c.Name,
            Description = c.Description,
            Status = c.Status.Name,
            StatusDisplayName = c.Status.DisplayName,
            CampaignType = c.CampaignType.Name,
            CampaignTypeDisplayName = c.CampaignType.DisplayName,
            DiscountType = c.DiscountType.Name,
            DiscountTypeDisplayName = c.DiscountType.DisplayName,
            TargetAudience = c.TargetAudience.Name,
            TargetAudienceDisplayName = c.TargetAudience.DisplayName,
            DiscountValue = c.DiscountValue,
            MaxDiscountAmount = c.MaxDiscountAmount,
            MinTransactionAmount = c.MinTransactionAmount,
            PointsMultiplier = c.PointsMultiplier,
            TotalBudget = c.TotalBudget,
            UsedBudget = c.UsedBudget,
            RemainingBudget = c.RemainingBudget,
            MaxUsageCount = c.MaxUsageCount,
            CurrentUsageCount = c.CurrentUsageCount,
            RemainingUsageCount = c.RemainingUsageCount,
            MaxUsagePerCustomer = c.MaxUsagePerCustomer,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            IsCurrentlyActive = c.IsCurrentlyActive,
            IsAllMerchants = c.IsAllMerchants,
            AllowedMerchantIds = c.AllowedMerchantIds,
            ApprovedBy = c.ApprovedBy,
            ApprovedAt = c.ApprovedAt,
            CreatedAt = c.CreatedAt,
            RuleCount = c.Rules.Count
        };
    }
}