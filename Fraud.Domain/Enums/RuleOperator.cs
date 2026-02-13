namespace Fraud.Domain.Enums;

/// <summary>
/// Kural parametresi karşılaştırma operatörü.
/// PayGuard'daki Operator entity karşılığı.
/// </summary>
public enum RuleOperator
{
    Equals = 1,
    NotEquals = 2,
    GreaterThan = 3,
    GreaterThanOrEqual = 4,
    LessThan = 5,
    LessThanOrEqual = 6,
    Contains = 7,
    NotContains = 8,
    In = 9,
    NotIn = 10,
    Between = 11,
    IsNull = 12,
    IsNotNull = 13
}