using Campaign.Application.DTOs;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public class ApplyCampaignCommandHandler
    : IRequestHandler<ApplyCampaignCommand, Result<ApplyCampaignResultDto>>
{
    private readonly ICampaignRepository _repository;

    public ApplyCampaignCommandHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ApplyCampaignResultDto>> Handle(
        ApplyCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Kampanyayı bul
        var campaign = await _repository.GetByIdWithDetailsAsync(
            await GetCampaignIdByCodeAsync(dto.CampaignCode, cancellationToken),
            cancellationToken);

        if (campaign == null)
        {
            return new ApplyCampaignResultDto
            {
                IsApplied = false,
                CampaignCode = dto.CampaignCode,
                CampaignName = "",
                OriginalAmount = dto.TransactionAmount,
                DiscountAmount = 0,
                FinalAmount = dto.TransactionAmount,
                PointsEarned = 0,
                ErrorMessage = "Kampanya bulunamadı"
            };
        }

        // Müşteri kullanım kontrolü
        if (campaign.MaxUsagePerCustomer.HasValue)
        {
            var customerUsageCount = await _repository.GetUsageCountByCustomerAsync(
                campaign.Id, dto.CardNumberMasked, cancellationToken);

            if (customerUsageCount >= campaign.MaxUsagePerCustomer.Value)
            {
                return new ApplyCampaignResultDto
                {
                    IsApplied = false,
                    CampaignCode = dto.CampaignCode,
                    CampaignName = campaign.Name,
                    OriginalAmount = dto.TransactionAmount,
                    DiscountAmount = 0,
                    FinalAmount = dto.TransactionAmount,
                    PointsEarned = 0,
                    ErrorMessage = "Müşteri kullanım limiti doldu"
                };
            }
        }

        // Kampanyayı uygula
        var usageResult = campaign.Use(
            dto.TransactionId,
            dto.CardNumberMasked,
            dto.MerchantId,
            dto.MerchantCode,
            dto.TransactionAmount,
            dto.MCC,
            dto.CardBIN);

        if (usageResult.IsFailure)
        {
            return new ApplyCampaignResultDto
            {
                IsApplied = false,
                CampaignCode = dto.CampaignCode,
                CampaignName = campaign.Name,
                OriginalAmount = dto.TransactionAmount,
                DiscountAmount = 0,
                FinalAmount = dto.TransactionAmount,
                PointsEarned = 0,
                ErrorMessage = usageResult.Error
            };
        }

        var usage = usageResult.Value!;

        // Kaydet
        await _repository.UpdateAsync(campaign, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new ApplyCampaignResultDto
        {
            IsApplied = true,
            CampaignCode = dto.CampaignCode,
            CampaignName = campaign.Name,
            OriginalAmount = usage.OriginalAmount,
            DiscountAmount = usage.DiscountAmount,
            FinalAmount = usage.FinalAmount,
            PointsEarned = usage.PointsEarned,
            ErrorMessage = null
        };
    }

    private async Task<Guid> GetCampaignIdByCodeAsync(string campaignCode, CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByCampaignCodeAsync(campaignCode, cancellationToken);
        return campaign?.Id ?? Guid.Empty;
    }
}