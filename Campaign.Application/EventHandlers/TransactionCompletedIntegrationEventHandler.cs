using Campaign.Application.Services;
using CardMerchantSystem.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Campaign.Application.EventHandlers;

/// <summary>
/// Transaction tamamlandığında kampanya kazanımları hesaplanır.
/// Post-Transaction: İşlem onaylandıktan sonra puan/cashback verilir.
/// </summary>
public class TransactionCompletedIntegrationEventHandler
    : INotificationHandler<TransactionCompletedIntegrationEvent>
{
    private readonly ICampaignApplicationService _campaignService;
    private readonly ILogger<TransactionCompletedIntegrationEventHandler> _logger;

    public TransactionCompletedIntegrationEventHandler(
        ICampaignApplicationService campaignService,
        ILogger<TransactionCompletedIntegrationEventHandler> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    public async Task Handle(TransactionCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "🎯 [Campaign] Kampanya hesaplama başladı - TransactionId: {TransactionId}, MerchantCode: {MerchantCode}, Amount: {Amount}",
            notification.TransactionId,
            notification.MerchantCode,
            notification.Amount);

        try
        {
            // Kampanya uygulama isteği oluştur
            var request = new CampaignApplicationRequest
            {
                TransactionId = notification.TransactionId,
                ReferenceNumber = notification.ReferenceNumber,
                CardNumberMasked = notification.CardNumberMasked,
                CardBin = ExtractCardBin(notification.CardNumberMasked),
                MerchantId = notification.MerchantId,
                MerchantCode = notification.MerchantCode,
                MCC = null, // TODO: MCC bilgisi event'e eklenebilir
                TransactionAmount = notification.Amount,
                Currency = "TRY"
            };

            // Uygun kampanyaları bul ve uygula
            var result = await _campaignService.ApplyEligibleCampaignsAsync(request, cancellationToken);

            if (result.IsSuccess && result.Value!.CampaignsApplied > 0)
            {
                _logger.LogInformation(
                    "✅ [Campaign] {Count} kampanya uygulandı - TransactionId: {TransactionId}, " +
                    "TotalDiscount: {Discount}, TotalPoints: {Points}, TotalCashback: {Cashback}",
                    result.Value.CampaignsApplied,
                    notification.TransactionId,
                    result.Value.TotalDiscount,
                    result.Value.TotalPointsEarned,
                    result.Value.TotalCashback);

                // Her uygulanan kampanyayı logla
                foreach (var benefit in result.Value.AppliedBenefits)
                {
                    _logger.LogDebug(
                        "  📌 {CampaignCode}: {BenefitType} - Discount={Discount}, Points={Points}, Cashback={Cashback}",
                        benefit.CampaignCode,
                        benefit.BenefitType,
                        benefit.DiscountAmount,
                        benefit.PointsEarned,
                        benefit.CashbackAmount);
                }
            }
            else if (result.IsSuccess)
            {
                _logger.LogDebug(
                    "ℹ️ [Campaign] Uygun kampanya bulunamadı - TransactionId: {TransactionId}",
                    notification.TransactionId);
            }
            else
            {
                _logger.LogWarning(
                    "⚠️ [Campaign] Kampanya uygulama hatası - TransactionId: {TransactionId}, Error: {Error}",
                    notification.TransactionId,
                    result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ [Campaign] Kampanya işleme hatası - TransactionId: {TransactionId}",
                notification.TransactionId);

            // Kampanya hatası transaction'ı etkilememeli
            // Hata loglayıp devam ediyoruz
        }
    }

    /// <summary>
    /// Kart numarasından BIN (ilk 6 hane) çıkarır
    /// </summary>
    private static string? ExtractCardBin(string cardNumberMasked)
    {
        if (string.IsNullOrEmpty(cardNumberMasked) || cardNumberMasked.Length < 6)
            return null;

        // Masked format: 4111****1234 veya 411111******1234
        var digitsOnly = new string(cardNumberMasked.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 6 ? digitsOnly[..6] : null;
    }
}