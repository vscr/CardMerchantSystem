using Fraud.Application.Services;
using Transaction.Domain.Enums;
using Transaction.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.Extensions.Logging;
using FraudModuleRequest = Fraud.Application.Models.FraudCheckRequest;

namespace Transaction.Infrastructure.Services;

/// <summary>
/// Transaction modülü ↔ Fraud modülü köprüsü.
/// 
/// Öncesi: Hardcoded kurallar (tutar > 50K, gece saati vb.)
/// Şimdi: Gerçek Fraud Engine'e delege ediyor.
/// 
/// Akış:
/// ProcessTransactionCommand → IFraudService.CheckFraudAsync()
///   → IFraudEngine.CheckOnlineAsync()
///     → Senaryo yükleme → Kural evaluate → HitScenario → Alert
///   ← FraudCheckResult (Status, Score, ResponseCode)
/// ← Transaction approve/decline
/// </summary>
public class FraudService : IFraudService
{
    private readonly IFraudEngine _fraudEngine;
    private readonly ILogger<FraudService> _logger;

    public FraudService(IFraudEngine fraudEngine, ILogger<FraudService> logger)
    {
        _fraudEngine = fraudEngine;
        _logger = logger;
    }

    public async Task<Result<FraudCheckResponse>> CheckFraudAsync(
        FraudCheckRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Transaction request → Fraud Engine request mapping
            var engineRequest = MapToEngineRequest(request);

            // 2. Fraud Engine çağır (Online — max 100ms hedef)
            var engineResult = await _fraudEngine.CheckOnlineAsync(engineRequest, cancellationToken);

            // 3. Fraud Engine result → Transaction response mapping
            var response = MapToResponse(engineResult);

            _logger.LogInformation(
                "Fraud check tamamlandı: Card={Card}, Score={Score}, Result={Result}, Süre={Ms}ms",
                request.CardNumberMasked, response.Score, response.Result.Name,
                engineResult.TotalExecutionTimeMs);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fraud check hatası: Card={Card}, Amount={Amount}",
                request.CardNumberMasked, request.Amount);

            // Fraud engine hata verirse işlemi geçir (fail-open)
            // Production'da fail-close da tercih edilebilir
            return Result.Success(new FraudCheckResponse
            {
                Result = FraudCheckResult.Pass,
                Score = 0,
                Reasons = new List<string> { "Fraud servisi geçici olarak kullanılamıyor" }
            });
        }
    }

    /// <summary>
    /// Transaction modülünün request'ini Fraud Engine formatına çevirir.
    /// PayGuard'daki TransactionInputParameters mapping karşılığı.
    /// </summary>
    private static FraudModuleRequest MapToEngineRequest(FraudCheckRequest request)
    {
        return new FraudModuleRequest
        {
            TransactionId = Guid.NewGuid(),
            TransactionDate = request.TransactionTime,
            TransactionHour = request.TransactionTime.Hour,

            // Tutar
            OriginalAmount = request.Amount,
            OriginalCurrencyCode = "TRY",

            // Kart
            MaskedCardNo = request.CardNumberMasked,

            // Üye İşyeri
            MerchantId = request.MerchantCode,
            TerminalId = request.TerminalCode,

            // POS
            ChannelType = request.DeviceId != null ? "ECOM" : "POS",
        };
    }

    /// <summary>
    /// Fraud Engine sonucunu Transaction modülünün anlayacağı formata çevirir.
    /// 
    /// Mapping:
    ///   FraudStatus.Clean       → FraudCheckResult.Pass
    ///   FraudStatus.Suspicious  → FraudCheckResult.Review (Score < 80)
    ///   FraudStatus.Fraudulent  → FraudCheckResult.Reject (Score >= 80)
    /// </summary>
    private static FraudCheckResponse MapToResponse(Fraud.Application.Models.FraudCheckResult engineResult)
    {
        var result = engineResult.Status switch
        {
            Fraud.Domain.Enums.FraudStatus.Clean => FraudCheckResult.Pass,
            Fraud.Domain.Enums.FraudStatus.Suspicious => FraudCheckResult.Review,
            Fraud.Domain.Enums.FraudStatus.Fraudulent => FraudCheckResult.Reject,
            Fraud.Domain.Enums.FraudStatus.SimulationHit => FraudCheckResult.Pass, // Simülasyon → geçir
            _ => FraudCheckResult.Pass
        };

        var reasons = engineResult.HitScenarios
            .Select(h => $"[{h.ScenarioName}] Score:{h.Score} Code:{h.FraudResponseCode}{(h.IsSimulation ? " (SIM)" : "")}")
            .ToList();

        return new FraudCheckResponse
        {
            Result = result,
            Score = engineResult.TotalScore,
            Reasons = reasons
        };
    }
}