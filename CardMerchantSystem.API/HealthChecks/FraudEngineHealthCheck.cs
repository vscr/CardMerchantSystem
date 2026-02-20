using Fraud.Domain.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CardMerchantSystem.API.HealthChecks;

/// <summary>
/// Fraud Engine sağlık kontrolü.
/// Aktif senaryo ve kural sayısını kontrol eder.
/// Senaryo yoksa fraud engine boşta çalışır — risk!
/// </summary>
public class FraudEngineHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FraudEngineHealthCheck(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var scenarioRepo = scope.ServiceProvider.GetRequiredService<IFraudScenarioRepository>();
            var ruleRepo = scope.ServiceProvider.GetRequiredService<IFraudRuleRepository>();

            var scenarios = await scenarioRepo.GetAllAsync(ct);
            var rules = await ruleRepo.GetAllActiveAsync(ct);

            var activeScenarios = scenarios.Count(s => s.IsActive);
            var onlineScenarios = scenarios.Count(s => s.IsActive &&
                (s.CheckMode == Fraud.Domain.Enums.FraudCheckMode.Online ||
                 s.CheckMode == Fraud.Domain.Enums.FraudCheckMode.Both));

            if (activeScenarios == 0)
                return HealthCheckResult.Unhealthy("Aktif fraud senaryosu yok — tüm işlemler kontrolsüz geçer!");

            if (onlineScenarios == 0)
                return HealthCheckResult.Degraded("Online fraud senaryosu yok — real-time kontrol devre dışı");

            return HealthCheckResult.Healthy(
                $"Fraud Engine OK — {activeScenarios} senaryo ({onlineScenarios} online), {rules.Count} kural");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Fraud Engine kontrol hatası", ex);
        }
    }
}