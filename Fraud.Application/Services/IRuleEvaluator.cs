using Fraud.Application.Models;
using Fraud.Domain.Entities;

namespace Fraud.Application.Services;

/// <summary>
/// Tekil kural değerlendirici.
/// PayGuard'daki IRuleExecutor karşılığı.
/// Her rule type için ayrı implementasyon (Strategy pattern).
/// </summary>
public interface IRuleEvaluator
{
    /// <summary>Kural tetiklendi mi?</summary>
    Task<bool> EvaluateAsync(FraudRule rule, FraudCheckRequest request, CancellationToken ct = default);
}