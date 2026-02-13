using Fraud.Application.Models;
using Fraud.Domain.Enums;

namespace Fraud.Application.Services;

/// <summary>
/// Fraud motoru ana interface'i.
/// PayGuard'daki IScenarioProcessor + IPayGRulesEngineService karşılığı.
/// </summary>
public interface IFraudEngine
{
    /// <summary>
    /// Online fraud kontrolü — provizyon anında çalışır.
    /// Max 100ms içinde sonuç dönmeli.
    /// </summary>
    Task<FraudCheckResult> CheckOnlineAsync(FraudCheckRequest request, CancellationToken ct = default);

    /// <summary>
    /// Offline fraud kontrolü — işlem sonrası batch çalışır.
    /// Daha derin analiz, süre kısıtı yok.
    /// </summary>
    Task<FraudCheckResult> CheckOfflineAsync(FraudCheckRequest request, CancellationToken ct = default);
}