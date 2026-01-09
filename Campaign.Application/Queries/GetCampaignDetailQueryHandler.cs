using Campaign.Application.DTOs;
using Campaign.Domain.Entities;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public class GetCampaignDetailQueryHandler
    : IRequestHandler<GetCampaignDetailQuery, Result<CampaignDetailDto>>
{
    private readonly ICampaignRepository _repository;

    public GetCampaignDetailQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CampaignDetailDto>> Handle(
        GetCampaignDetailQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (campaign == null)
            return Result.Failure<CampaignDetailDto>("Kampanya bulunamadı", ErrorCodes.NotFound);

        return new CampaignDetailDto
        {
            Campaign = MapToDto(campaign),
            Rules = campaign.Rules.Select(MapRuleToDto).ToList(),
            RecentUsages = campaign.Usages.OrderByDescending(u => u.UsedAt).Take(20).Select(MapUsageToDto).ToList()
        };
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

    private static CampaignRuleDto MapRuleToDto(CampaignRule rule)
    {
        return new CampaignRuleDto
        {
            Id = rule.Id,
            RuleName = rule.RuleName,
            RuleType = rule.RuleType,
            Operator = rule.Operator,
            Value = rule.Value,
            IsActive = rule.IsActive
        };
    }

    private static CampaignUsageDto MapUsageToDto(CampaignUsage usage)
    {
        return new CampaignUsageDto
        {
            Id = usage.Id,
            TransactionId = usage.TransactionId,
            CardNumberMasked = usage.CardNumberMasked,
            MerchantCode = usage.MerchantCode,
            OriginalAmount = usage.OriginalAmount,
            DiscountAmount = usage.DiscountAmount,
            FinalAmount = usage.FinalAmount,
            PointsEarned = usage.PointsEarned,
            UsedAt = usage.UsedAt
        };
    }
}