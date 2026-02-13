using Fraud.Application.Models;
using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Fraud.Application.Services;

/// <summary>
/// Kural değerlendirici. PayGuard'daki IRuleExecutor implementasyonları karşılığı.
/// Rule type'a göre farklı evaluation stratejisi uygular.
/// </summary>
public class RuleEvaluator : IRuleEvaluator
{
    private readonly IFraudBlacklistRepository _blacklistRepo;
    private readonly IHitScenarioRepository _hitScenarioRepo;
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly ILogger<RuleEvaluator> _logger;

    public RuleEvaluator(
        IFraudBlacklistRepository blacklistRepo,
        IHitScenarioRepository hitScenarioRepo,
        ICardFraudProfileRepository profileRepo,
        ILogger<RuleEvaluator> logger)
    {
        _blacklistRepo = blacklistRepo;
        _hitScenarioRepo = hitScenarioRepo;
        _profileRepo = profileRepo;
        _logger = logger;
    }

    public async Task<bool> EvaluateAsync(FraudRule rule, FraudCheckRequest request, CancellationToken ct = default)
    {
        if (!rule.IsActive) return false;

        return rule.RuleType switch
        {
            FraudRuleType.Simple => EvaluateSimple(rule, request),
            FraudRuleType.Complex => EvaluateComplex(rule, request),
            FraudRuleType.Periodic => await EvaluatePeriodicAsync(rule, request, ct),
            FraudRuleType.Linked => await EvaluateLinkedAsync(rule, request, ct),
            _ => throw new InvalidOperationException($"Bilinmeyen kural tipi: {rule.RuleType}")
        };
    }

    /// <summary>
    /// Simple: Tek koşul kontrolü
    /// Örnek: OriginalAmount > 10000
    /// </summary>
    private bool EvaluateSimple(FraudRule rule, FraudCheckRequest request)
    {
        var condition = rule.Conditions.FirstOrDefault();
        if (condition == null) return false;

        return EvaluateCondition(condition, request);
    }

    /// <summary>
    /// Complex: Birden fazla koşul AND/OR ile bağlanır
    /// Örnek: Amount > 5000 AND Country != TR AND MCC IN (5411,5412)
    /// </summary>
    private bool EvaluateComplex(FraudRule rule, FraudCheckRequest request)
    {
        if (!rule.Conditions.Any()) return false;

        var results = rule.Conditions
            .OrderBy(c => c.OrderIndex)
            .Select(c => EvaluateCondition(c, request))
            .ToList();

        return rule.LogicalOperator == LogicalOperator.And
            ? results.All(r => r)
            : results.Any(r => r);
    }

    /// <summary>
    /// Periodic: Zaman penceresi içinde toplam/sayım kontrolü
    /// Örnek: Son 1 saatte aynı karttan 5+ işlem
    /// CardFraudProfile üzerinden hızlı kontrol yapılır.
    /// </summary>
    private async Task<bool> EvaluatePeriodicAsync(FraudRule rule, FraudCheckRequest request, CancellationToken ct)
    {
        if (!rule.PeriodMinutes.HasValue || !rule.PeriodThreshold.HasValue)
            return false;

        var profile = await _profileRepo.GetByCardNoAsync(request.MaskedCardNo, ct);
        if (profile == null) return false;

        var function = rule.PeriodFunction?.ToUpperInvariant() ?? "COUNT";
        var groupBy = rule.PeriodGroupBy?.ToUpperInvariant() ?? "CARD";
        var threshold = rule.PeriodThreshold.Value;

        decimal actualValue = (function, rule.PeriodMinutes.Value) switch
        {
            ("COUNT", <= 60) => profile.Last1HourTxCount,
            ("COUNT", <= 1440) => profile.Last24HourTxCount,
            ("COUNT", _) => profile.Last7DayTxCount,
            ("SUM", <= 60) => profile.Last1HourTxAmount,
            ("SUM", <= 1440) => profile.Last24HourTxAmount,
            ("SUM", _) => profile.TotalTransactionAmount,
            ("DISTINCT_COUNTRY", _) => profile.Last24HourDistinctCountryCount,
            ("DISTINCT_MERCHANT", _) => profile.Last24HourDistinctMerchantCount,
            _ => 0
        };

        var result = actualValue >= threshold;

        if (result)
        {
            _logger.LogDebug(
                "Periodic rule tetiklendi: {RuleName}, Func={Func}, Actual={Actual}, Threshold={Threshold}",
                rule.Name, function, actualValue, threshold);
        }

        return result;
    }

    /// <summary>
    /// Linked: SQL script bazlı (gelecekte Dapper ile)
    /// Şimdilik placeholder — production'da RuleDapperRepository kullanılır.
    /// </summary>
    private async Task<bool> EvaluateLinkedAsync(FraudRule rule, FraudCheckRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(rule.SqlScript))
            return false;

        // TODO: Dapper ile SQL çalıştır
        // PayGuard'daki RuleDapperRepository.ExecuteLinkedRule karşılığı
        _logger.LogDebug("Linked rule placeholder: {RuleName}", rule.Name);

        await Task.CompletedTask;
        return false;
    }

    /// <summary>
    /// Tekil koşul değerlendirme.
    /// Request'ten parametre adına göre değer çeker, operatöre göre karşılaştırır.
    /// </summary>
    private bool EvaluateCondition(FraudRuleCondition condition, FraudCheckRequest request)
    {
        var paramValue = GetParameterValue(condition.ParameterName, request);

        if (condition.Operator == RuleOperator.IsNull) return paramValue == null;
        if (condition.Operator == RuleOperator.IsNotNull) return paramValue != null;
        if (paramValue == null) return false;

        return condition.Operator switch
        {
            RuleOperator.Equals => CompareEquals(paramValue, condition.Value),
            RuleOperator.NotEquals => !CompareEquals(paramValue, condition.Value),
            RuleOperator.GreaterThan => CompareNumeric(paramValue, condition.Value, (a, b) => a > b),
            RuleOperator.GreaterThanOrEqual => CompareNumeric(paramValue, condition.Value, (a, b) => a >= b),
            RuleOperator.LessThan => CompareNumeric(paramValue, condition.Value, (a, b) => a < b),
            RuleOperator.LessThanOrEqual => CompareNumeric(paramValue, condition.Value, (a, b) => a <= b),
            RuleOperator.Contains => paramValue.ToString()!.Contains(condition.Value, StringComparison.OrdinalIgnoreCase),
            RuleOperator.NotContains => !paramValue.ToString()!.Contains(condition.Value, StringComparison.OrdinalIgnoreCase),
            RuleOperator.In => condition.Value.Split(',').Contains(paramValue.ToString(), StringComparer.OrdinalIgnoreCase),
            RuleOperator.NotIn => !condition.Value.Split(',').Contains(paramValue.ToString(), StringComparer.OrdinalIgnoreCase),
            RuleOperator.Between => EvaluateBetween(paramValue, condition.Value, condition.SecondValue),
            _ => false
        };
    }

    /// <summary>
    /// Reflection ile FraudCheckRequest'ten parametre değerini çeker.
    /// PayGuard'daki TransactionInputParameters mapping karşılığı.
    /// </summary>
    private object? GetParameterValue(string parameterName, FraudCheckRequest request)
    {
        var property = typeof(FraudCheckRequest).GetProperty(parameterName,
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.IgnoreCase);

        return property?.GetValue(request);
    }

    private bool CompareEquals(object actual, string expected)
    {
        return string.Equals(actual.ToString(), expected, StringComparison.OrdinalIgnoreCase);
    }

    private bool CompareNumeric(object actual, string expected, Func<decimal, decimal, bool> comparison)
    {
        if (decimal.TryParse(actual.ToString(), out var actualNum) &&
            decimal.TryParse(expected, out var expectedNum))
        {
            return comparison(actualNum, expectedNum);
        }
        return false;
    }

    private bool EvaluateBetween(object actual, string lower, string? upper)
    {
        if (upper == null) return false;

        if (decimal.TryParse(actual.ToString(), out var actualNum) &&
            decimal.TryParse(lower, out var lowerNum) &&
            decimal.TryParse(upper, out var upperNum))
        {
            return actualNum >= lowerNum && actualNum <= upperNum;
        }
        return false;
    }
}