using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.Extensions.Logging;

namespace Campaign.Application.Services;

/// <summary>
/// Kampanya uygulama servisi.
/// Transaction tamamlandıktan sonra kampanya kazanımlarını hesaplar.
/// </summary>
public class CampaignApplicationService : ICampaignApplicationService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<CampaignApplicationService> _logger;

    public CampaignApplicationService(
        ICampaignRepository campaignRepository,
        ILogger<CampaignApplicationService> logger)
    {
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<CampaignApplicationResult>> ApplyEligibleCampaignsAsync(
        CampaignApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Kampanya uygulama başladı - TransactionId: {TransactionId}, MerchantId: {MerchantId}, Amount: {Amount}",
            request.TransactionId, request.MerchantId, request.TransactionAmount);

        var result = new CampaignApplicationResult
        {
            TransactionId = request.TransactionId,
            AppliedBenefits = new List<CampaignBenefitDto>()
        };

        try
        {
            // 1. Aktif kampanyaları getir (rules ile birlikte)
            var activeCampaigns = await _campaignRepository.GetActiveAsync(cancellationToken);

            if (!activeCampaigns.Any())
            {
                _logger.LogDebug("Aktif kampanya bulunamadı");
                return result;
            }

            _logger.LogDebug("{Count} aktif kampanya bulundu", activeCampaigns.Count);

            decimal totalDiscount = 0;
            int totalPoints = 0;
            decimal totalCashback = 0;

            // 2. Her kampanyayı değerlendir
            foreach (var campaign in activeCampaigns)
            {
                // Kampanyayı detaylı getir (rules dahil)
                var campaignWithDetails = await _campaignRepository.GetByIdWithDetailsAsync(campaign.Id, cancellationToken);
                if (campaignWithDetails == null) continue;

                // Müşteri başına kullanım limiti kontrolü
                if (campaignWithDetails.MaxUsagePerCustomer.HasValue)
                {
                    var customerUsageCount = await _campaignRepository.GetUsageCountByCustomerAsync(
                        campaign.Id, request.CardNumberMasked, cancellationToken);

                    if (customerUsageCount >= campaignWithDetails.MaxUsagePerCustomer.Value)
                    {
                        _logger.LogDebug(
                            "Kampanya {CampaignCode} müşteri kullanım limiti aşıldı ({Count}/{Max})",
                            campaign.CampaignCode, customerUsageCount, campaignWithDetails.MaxUsagePerCustomer.Value);
                        continue;
                    }
                }

                // Kampanyayı uygula
                var usageResult = campaignWithDetails.Use(
                    request.TransactionId,
                    request.CardNumberMasked,
                    request.MerchantId,
                    request.MerchantCode,
                    request.TransactionAmount,
                    request.MCC,
                    request.CardBin);

                if (usageResult.IsFailure)
                {
                    _logger.LogDebug(
                        "Kampanya {CampaignCode} uygulanamadı: {Error}",
                        campaign.CampaignCode, usageResult.Error);
                    continue;
                }

                var usage = usageResult.Value!;

                // Kazanım tipini belirle
                var benefitType = GetBenefitType(campaignWithDetails);
                var benefit = new CampaignBenefitDto
                {
                    CampaignId = campaign.Id,
                    CampaignCode = campaign.CampaignCode,
                    CampaignName = campaign.Name,
                    BenefitType = benefitType,
                    DiscountAmount = usage.DiscountAmount,
                    PointsEarned = usage.PointsEarned,
                    CashbackAmount = CalculateCashback(campaignWithDetails, usage.DiscountAmount),
                    UsageId = usage.Id
                };

                result.AppliedBenefits.Add(benefit);

                totalDiscount += benefit.DiscountAmount;
                totalPoints += benefit.PointsEarned;
                totalCashback += benefit.CashbackAmount;

                // Kampanyayı kaydet (UsedBudget ve CurrentUsageCount güncellenmiş)
                await _campaignRepository.UpdateAsync(campaignWithDetails, cancellationToken);

                _logger.LogInformation(
                    "✅ Kampanya uygulandı - {CampaignCode}: Discount={Discount}, Points={Points}, Cashback={Cashback}",
                    campaign.CampaignCode, benefit.DiscountAmount, benefit.PointsEarned, benefit.CashbackAmount);
            }

            // 3. Tüm değişiklikleri kaydet
            await _campaignRepository.SaveChangesAsync(cancellationToken);

            result = result with
            {
                TotalDiscount = totalDiscount,
                TotalPointsEarned = totalPoints,
                TotalCashback = totalCashback
            };

            _logger.LogInformation(
                "Kampanya uygulama tamamlandı - TransactionId: {TransactionId}, " +
                "Uygulanan: {Count}, Discount: {Discount}, Points: {Points}, Cashback: {Cashback}",
                request.TransactionId, result.CampaignsApplied, totalDiscount, totalPoints, totalCashback);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kampanya uygulama hatası - TransactionId: {TransactionId}", request.TransactionId);
            return Result.Failure<CampaignApplicationResult>($"Kampanya uygulama hatası: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<CampaignBenefitDto>> ApplyCampaignAsync(
        Guid campaignId,
        CampaignApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdWithDetailsAsync(campaignId, cancellationToken);
        if (campaign == null)
            return Result.Failure<CampaignBenefitDto>("Kampanya bulunamadı");

        // Müşteri başına kullanım limiti kontrolü
        if (campaign.MaxUsagePerCustomer.HasValue)
        {
            var customerUsageCount = await _campaignRepository.GetUsageCountByCustomerAsync(
                campaignId, request.CardNumberMasked, cancellationToken);

            if (customerUsageCount >= campaign.MaxUsagePerCustomer.Value)
                return Result.Failure<CampaignBenefitDto>("Kampanya müşteri kullanım limiti aşıldı");
        }

        var usageResult = campaign.Use(
            request.TransactionId,
            request.CardNumberMasked,
            request.MerchantId,
            request.MerchantCode,
            request.TransactionAmount,
            request.MCC,
            request.CardBin);

        if (usageResult.IsFailure)
            return Result.Failure<CampaignBenefitDto>(usageResult.Error!);

        var usage = usageResult.Value!;

        await _campaignRepository.UpdateAsync(campaign, cancellationToken);
        await _campaignRepository.SaveChangesAsync(cancellationToken);

        return new CampaignBenefitDto
        {
            CampaignId = campaign.Id,
            CampaignCode = campaign.CampaignCode,
            CampaignName = campaign.Name,
            BenefitType = GetBenefitType(campaign),
            DiscountAmount = usage.DiscountAmount,
            PointsEarned = usage.PointsEarned,
            CashbackAmount = CalculateCashback(campaign, usage.DiscountAmount),
            UsageId = usage.Id
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CampaignPreviewDto>> GetEligibleCampaignsAsync(
        string cardNumberMasked,
        Guid merchantId,
        decimal transactionAmount,
        CancellationToken cancellationToken = default)
    {
        var result = new List<CampaignPreviewDto>();

        var activeCampaigns = await _campaignRepository.GetActiveAsync(cancellationToken);

        foreach (var campaign in activeCampaigns)
        {
            // Basit uygunluk kontrolü
            if (!campaign.IsCurrentlyActive)
                continue;

            if (campaign.MinTransactionAmount.HasValue && transactionAmount < campaign.MinTransactionAmount.Value)
                continue;

            if (!campaign.IsAllMerchants && campaign.AllowedMerchantIds != null &&
                !campaign.AllowedMerchantIds.Contains(merchantId))
                continue;

            if (campaign.MaxUsageCount.HasValue && campaign.CurrentUsageCount >= campaign.MaxUsageCount.Value)
                continue;

            var estimatedDiscount = campaign.CalculateDiscount(transactionAmount);

            result.Add(new CampaignPreviewDto
            {
                CampaignId = campaign.Id,
                CampaignCode = campaign.CampaignCode,
                CampaignName = campaign.Name,
                Description = campaign.Description,
                BenefitType = GetBenefitType(campaign),
                EstimatedDiscount = estimatedDiscount,
                EstimatedPoints = campaign.CampaignType.IsPointsBased
                    ? (int)(transactionAmount * campaign.PointsMultiplier)
                    : 0,
                EstimatedCashback = campaign.CampaignType == CampaignType.Cashback
                    ? estimatedDiscount
                    : 0,
                EndDate = campaign.EndDate
            });
        }

        return result.OrderByDescending(x => x.EstimatedDiscount + x.EstimatedCashback + x.EstimatedPoints)
            .ToList();
    }

    private static string GetBenefitType(CampaignAggregate campaign)
    {
        return campaign.CampaignType.Name switch
        {
            "Points" => "Points",
            "Cashback" => "Cashback",
            _ => "Discount"
        };
    }

    private static decimal CalculateCashback(CampaignAggregate campaign, decimal discountAmount)
    {
        // Cashback kampanyası ise discount tutarını cashback olarak döndür
        if (campaign.CampaignType == CampaignType.Cashback)
            return discountAmount;

        return 0;
    }
}