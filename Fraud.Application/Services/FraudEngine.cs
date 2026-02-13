using System.Diagnostics;
using Fraud.Application.Models;
using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Events;
using Fraud.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fraud.Application.Services;

/// <summary>
/// Fraud motoru implementasyonu.
/// PayGuard'daki CardScenarioProcessor karşılığı.
/// 
/// Akış:
/// 1. Aktif senaryoları yükle (cache'den)
/// 2. Her senaryo için:
///    a. Filtre kuralını çalıştır → TRUE ise senaryoyu ATLA
///    b. Ana kuralı çalıştır → TRUE ise fraud TESPİT EDİLDİ
///    c. HitScenario kaydı oluştur
///    d. FraudDetectedEvent fırlat
/// 3. En yüksek skorlu sonucu döndür
/// </summary>
public class FraudEngine : IFraudEngine
{
    private readonly IFraudScenarioRepository _scenarioRepo;
    private readonly IHitScenarioRepository _hitScenarioRepo;
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly IRuleEvaluator _ruleEvaluator;
    private readonly IMediator _mediator;
    private readonly ILogger<FraudEngine> _logger;

    public FraudEngine(
        IFraudScenarioRepository scenarioRepo,
        IHitScenarioRepository hitScenarioRepo,
        ICardFraudProfileRepository profileRepo,
        IRuleEvaluator ruleEvaluator,
        IMediator mediator,
        ILogger<FraudEngine> logger)
    {
        _scenarioRepo = scenarioRepo;
        _hitScenarioRepo = hitScenarioRepo;
        _profileRepo = profileRepo;
        _ruleEvaluator = ruleEvaluator;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<FraudCheckResult> CheckOnlineAsync(FraudCheckRequest request, CancellationToken ct = default)
    {
        return await ExecuteCheckAsync(request, FraudCheckMode.Online, ct);
    }

    public async Task<FraudCheckResult> CheckOfflineAsync(FraudCheckRequest request, CancellationToken ct = default)
    {
        return await ExecuteCheckAsync(request, FraudCheckMode.Offline, ct);
    }

    private async Task<FraudCheckResult> ExecuteCheckAsync(
        FraudCheckRequest request, FraudCheckMode mode, CancellationToken ct)
    {
        var totalStopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Fraud check başladı. TransactionId: {TxId}, Mode: {Mode}, Card: {Card}",
            request.TransactionId, mode, request.MaskedCardNo);

        // 1. Aktif senaryoları yükle
        var scenarios = await _scenarioRepo.GetActiveByModeAsync(mode, ct);
        var now = DateTime.UtcNow;
        var effectiveScenarios = scenarios
            .Where(s => s.IsEffective(now))
            .OrderBy(s => s.RunOrder)
            .ToList();

        if (effectiveScenarios.Count == 0)
        {
            totalStopwatch.Stop();
            return FraudCheckResult.Clean(request.TransactionId, totalStopwatch.ElapsedMilliseconds);
        }

        // 2. Kart profilini al veya oluştur
        var profile = await _profileRepo.GetOrCreateAsync(request.MaskedCardNo, ct);
        profile.RecordTransaction(
            request.OriginalAmount,
            request.MerchantCountryCode,
            request.MerchantId);

        // 3. Her senaryoyu çalıştır
        var hitScenarios = new List<HitScenarioResult>();
        var hitEntities = new List<HitScenario>();
        var highestScore = 0;
        var highestFraudResponseCode = "00";

        foreach (var scenario in effectiveScenarios)
        {
            var scenarioStopwatch = Stopwatch.StartNew();

            try
            {
                // 3a. Filtre kuralı — TRUE dönerse senaryoyu ATLA
                if (scenario.FilterRuleId.HasValue && scenario.FilterRule != null)
                {
                    var filterResult = await _ruleEvaluator.EvaluateAsync(
                        scenario.FilterRule, request, ct);

                    if (filterResult)
                    {
                        _logger.LogDebug(
                            "Senaryo filtrelendi: {ScenarioName}", scenario.Name);
                        continue;
                    }
                }

                // 3b. Ana kural — TRUE dönerse FRAUD TESPİT EDİLDİ
                var ruleResult = await _ruleEvaluator.EvaluateAsync(
                    scenario.Rule, request, ct);

                scenarioStopwatch.Stop();

                if (ruleResult)
                {
                    _logger.LogWarning(
                        "FRAUD TESPİT: Senaryo={ScenarioName}, Score={Score}, Card={Card}, Tx={TxId}",
                        scenario.Name, scenario.Score, request.MaskedCardNo, request.TransactionId);

                    // HitScenario kaydı
                    var hit = new HitScenario(
                        request.TransactionId, scenario.Id,
                        request.MaskedCardNo, request.MerchantId,
                        scenario.Score, scenario.FraudResponseCode,
                        mode == FraudCheckMode.Online, scenario.IsSimulation,
                        scenarioStopwatch.ElapsedMilliseconds);

                    hitEntities.Add(hit);

                    hitScenarios.Add(new HitScenarioResult
                    {
                        ScenarioId = scenario.Id,
                        ScenarioName = scenario.Name,
                        Score = scenario.Score,
                        FraudResponseCode = scenario.FraudResponseCode,
                        IsSimulation = scenario.IsSimulation,
                        ExecutionTimeMs = scenarioStopwatch.ElapsedMilliseconds
                    });

                    // En yüksek skorlu (simülasyon olmayan) senaryoyu belirle
                    if (!scenario.IsSimulation && scenario.Score > highestScore)
                    {
                        highestScore = scenario.Score;
                        highestFraudResponseCode = scenario.FraudResponseCode;
                    }

                    // Profil güncelle
                    profile.RecordHitScenario(scenario.Score);

                    // Domain event fırlat
                    await _mediator.Publish(new FraudDetectedEvent(
                        request.TransactionId, scenario.Id,
                        request.MaskedCardNo, request.MerchantId, request.MerchantName,
                        request.OriginalAmount, request.OriginalCurrencyCode,
                        scenario.Score, scenario.FraudResponseCode,
                        scenario.IsSimulation, mode == FraudCheckMode.Online,
                        scenarioStopwatch.ElapsedMilliseconds), ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Senaryo çalıştırma hatası: {ScenarioName}, Tx={TxId}",
                    scenario.Name, request.TransactionId);
            }
        }

        // 4. Kaydet
        if (hitEntities.Count > 0)
            await _hitScenarioRepo.AddRangeAsync(hitEntities, ct);

        await _profileRepo.UpdateAsync(profile, ct);

        // 5. Sonuç belirle
        totalStopwatch.Stop();

        var realHits = hitScenarios.Where(h => !h.IsSimulation).ToList();
        var status = realHits.Count == 0
            ? FraudStatus.Clean
            : highestScore >= 80
                ? FraudStatus.Fraudulent
                : FraudStatus.Suspicious;

        var result = new FraudCheckResult
        {
            TransactionId = request.TransactionId,
            Status = status,
            FraudResponseCode = highestFraudResponseCode,
            TotalScore = highestScore,
            HitScenarioCount = hitScenarios.Count,
            HitScenarios = hitScenarios,
            TotalExecutionTimeMs = totalStopwatch.ElapsedMilliseconds
        };

        _logger.LogInformation(
            "Fraud check tamamlandı. Tx={TxId}, Status={Status}, Score={Score}, HitCount={Hits}, Süre={Ms}ms",
            request.TransactionId, status, highestScore, hitScenarios.Count,
            totalStopwatch.ElapsedMilliseconds);

        return result;
    }
}