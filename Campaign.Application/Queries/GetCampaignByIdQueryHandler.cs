using Campaign.Application.DTOs;
using Campaign.Domain.Entities;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public class GetCampaignByIdQueryHandler
    : IRequestHandler<GetCampaignByIdQuery, Result<CampaignDto>>
{
    private readonly ICampaignRepository _repository;

    public GetCampaignByIdQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CampaignDto>> Handle(
        GetCampaignByIdQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (campaign == null)
            return Result.Failure<CampaignDto>("Kampanya bulunamadı", ErrorCodes.NotFound);

        return MapToDto(campaign);
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