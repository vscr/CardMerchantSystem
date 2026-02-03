using Campaign.Application.DTOs;
using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Campaign.Application.Queries;

/// <summary>
/// Sayfalı kampanya listesi query'si
/// </summary>
public record GetCampaignsPagedQuery(CampaignFilterDto Filter) : IRequest<PagedResponse<CampaignDto>>;

public class GetCampaignsPagedQueryHandler : IRequestHandler<GetCampaignsPagedQuery, PagedResponse<CampaignDto>>
{
    private readonly ICampaignRepository _repository;
    private readonly ILogger<GetCampaignsPagedQueryHandler> _logger;

    public GetCampaignsPagedQueryHandler(
        ICampaignRepository repository,
        ILogger<GetCampaignsPagedQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<CampaignDto>> Handle(
        GetCampaignsPagedQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching campaigns page {PageNumber} with size {PageSize}",
            request.Filter.PageNumber,
            request.Filter.PageSize);

        // Status enum'ı çözümle
        CampaignStatus? status = null;
        if (request.Filter.StatusId.HasValue)
        {
            status = CampaignStatus.FromId<CampaignStatus>(request.Filter.StatusId.Value);
        }

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Filter.PageNumber,
            request.Filter.PageSize,
            status,
            request.Filter.MerchantId,
            request.Filter.SearchTerm,
            request.Filter.StartDateFrom,
            request.Filter.StartDateTo,
            request.Filter.IsActive,
            request.Filter.SortBy,
            request.Filter.SortDescending,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        _logger.LogInformation(
            "Retrieved {Count} campaigns out of {Total}",
            dtos.Count,
            totalCount);

        return PagedResponse<CampaignDto>.Create(
            dtos,
            totalCount,
            request.Filter.PageNumber,
            request.Filter.PageSize);
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
            StatusId = c.Status.Id,
            StatusDisplayName = c.Status.DisplayName,
            CampaignType = c.CampaignType.Name,
            CampaignTypeId = c.CampaignType.Id,
            CampaignTypeDisplayName = c.CampaignType.DisplayName,
            DiscountType = c.DiscountType.Name,
            DiscountTypeId = c.DiscountType.Id,
            DiscountTypeDisplayName = c.DiscountType.DisplayName,
            TargetAudience = c.TargetAudience.Name,
            TargetAudienceId = c.TargetAudience.Id,
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