using Campaign.Application.DTOs;
using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record CreateCampaignCommand(CreateCampaignDto Dto) : IRequest<Result<CampaignDto>>;
public class CreateCampaignCommandHandler
    : IRequestHandler<CreateCampaignCommand, Result<CampaignDto>>
{
    private readonly ICampaignRepository _repository;

    public CreateCampaignCommandHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CampaignDto>> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Enum'ları bul
        var campaignType = CampaignType.FromId<CampaignType>(dto.CampaignTypeId);
        if (campaignType == null)
            return Result.Failure<CampaignDto>("Geçersiz kampanya tipi", ErrorCodes.ValidationError);

        var discountType = DiscountType.FromId<DiscountType>(dto.DiscountTypeId);
        if (discountType == null)
            return Result.Failure<CampaignDto>("Geçersiz indirim tipi", ErrorCodes.ValidationError);

        var targetAudience = TargetAudience.FromId<TargetAudience>(dto.TargetAudienceId);
        if (targetAudience == null)
            return Result.Failure<CampaignDto>("Geçersiz hedef kitle", ErrorCodes.ValidationError);

        // Kampanya oluştur
        var campaignResult = CampaignAggregate.Create(
            dto.Name,
            dto.Description,
            campaignType,
            discountType,
            targetAudience,
            dto.DiscountValue,
            dto.StartDate,
            dto.EndDate,
            dto.MaxDiscountAmount,
            dto.MinTransactionAmount,
            dto.TotalBudget,
            dto.MaxUsageCount,
            dto.MaxUsagePerCustomer,
            dto.PointsMultiplier);

        if (campaignResult.IsFailure)
            return Result.Failure<CampaignDto>(campaignResult.Error!, campaignResult.ErrorCode);

        var campaign = campaignResult.Value!;

        // Kaydet
        await _repository.AddAsync(campaign, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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