using Transaction.Domain.Enums;
using Transaction.Domain.Services;
using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Infrastructure.Services;

/// <summary>
/// Fraud kontrol servisi
/// Basit kural tabanlı fraud detection
/// </summary>
public class FraudService : IFraudService
{
    // Fraud kuralları için eşik değerleri
    private const decimal HIGH_AMOUNT_THRESHOLD = 50000m;
    private const int MAX_TRANSACTIONS_PER_HOUR = 10;
    private const int MAX_FRAUD_SCORE = 100;
    private const int FRAUD_REJECT_THRESHOLD = 80;
    private const int FRAUD_REVIEW_THRESHOLD = 50;

    public Task<Result<FraudCheckResponse>> CheckFraudAsync(FraudCheckRequest request, CancellationToken cancellationToken = default)
    {
        var score = 0;
        var reasons = new List<string>();

        // Kural 1: Yüksek tutar kontrolü
        if (request.Amount > HIGH_AMOUNT_THRESHOLD)
        {
            score += 30;
            reasons.Add($"Yüksek işlem tutarı: {request.Amount:N2} TRY");
        }

        // Kural 2: Gece yarısı işlem kontrolü (00:00 - 05:00)
        var hour = request.TransactionTime.Hour;
        if (hour >= 0 && hour < 5)
        {
            score += 20;
            reasons.Add("Gece saatlerinde işlem");
        }

        // Kural 3: Çok yüksek tutar (100K+)
        if (request.Amount > 100000m)
        {
            score += 40;
            reasons.Add("Çok yüksek işlem tutarı");
        }

        // Kural 4: Round amount kontrolü (tam sayı)
        if (request.Amount == Math.Floor(request.Amount) && request.Amount > 1000)
        {
            score += 10;
            reasons.Add("Yuvarlak tutar");
        }

        // Sonucu belirle
        FraudCheckResult result;
        if (score >= FRAUD_REJECT_THRESHOLD)
        {
            result = FraudCheckResult.Reject;
        }
        else if (score >= FRAUD_REVIEW_THRESHOLD)
        {
            result = FraudCheckResult.Review;
        }
        else
        {
            result = FraudCheckResult.Pass;
        }

        var response = new FraudCheckResponse
        {
            Result = result,
            Score = Math.Min(score, MAX_FRAUD_SCORE),
            Reasons = reasons
        };

        return Task.FromResult(Result.Success(response));
    }
}