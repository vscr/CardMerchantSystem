using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Kural koşulu. PayGuard'daki SubRule + SubRuleCondition karşılığı.
/// Bir FraudRule birden fazla condition içerebilir (AND/OR ile bağlanır).
/// 
/// Örnek:
/// ParameterName = "OriginalAmount", Operator = GreaterThan, Value = "10000"
/// ParameterName = "MerchantCountryCode", Operator = NotEquals, Value = "TR"
/// ParameterName = "Mcc", Operator = In, Value = "5411,5412,5413"
/// </summary>
public class FraudRuleCondition : Entity
{
    public Guid FraudRuleId { get; private set; }

    /// <summary>
    /// İşlem parametresi adı (TransactionInputParameters'daki alan adı).
    /// Örnek: OriginalAmount, MerchantCountryCode, Mcc, IsEmvTransaction
    /// </summary>
    public string ParameterName { get; private set; } = null!;

    /// <summary>
    /// Karşılaştırma operatörü
    /// </summary>
    public RuleOperator Operator { get; private set; }

    /// <summary>
    /// Karşılaştırma değeri (string olarak saklanır, runtime'da cast edilir).
    /// In/NotIn için virgülle ayrılmış değerler: "TR,US,GB"
    /// Between için: "1000|5000"
    /// </summary>
    public string Value { get; private set; } = null!;

    /// <summary>
    /// İkinci değer (Between operatörü için üst sınır)
    /// </summary>
    public string? SecondValue { get; private set; }

    /// <summary>
    /// Koşul sırası (evaluation order)
    /// </summary>
    public int OrderIndex { get; private set; }

    // Navigation
    public FraudRule FraudRule { get; private set; } = null!;

    private FraudRuleCondition() { } // EF Core

    public FraudRuleCondition(
        Guid fraudRuleId, string parameterName, RuleOperator @operator,
        string value, string? secondValue, int orderIndex)
    {
        FraudRuleId = fraudRuleId;
        ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        Operator = @operator;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        SecondValue = secondValue;
        OrderIndex = orderIndex;
    }

    public void Update(string parameterName, RuleOperator @operator, string value, string? secondValue)
    {
        ParameterName = parameterName;
        Operator = @operator;
        Value = value;
        SecondValue = secondValue;
    }
}