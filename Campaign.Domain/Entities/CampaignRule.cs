using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Entities;

/// <summary>
/// Kampanya kuralı
/// </summary>
public class CampaignRule : Entity
{
    public Guid CampaignId { get; private set; }
    public string RuleName { get; private set; } = null!;
    public string RuleType { get; private set; } = null!;
    public string Operator { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public bool IsActive { get; private set; }

    // EF Core için
    private CampaignRule() { }

    public static CampaignRule Create(
        Guid campaignId,
        string ruleName,
        string ruleType,
        string operatorType,
        string value)
    {
        return new CampaignRule
        {
            CampaignId = campaignId,
            RuleName = ruleName,
            RuleType = ruleType,
            Operator = operatorType,
            Value = value,
            IsActive = true
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Kuralı değerlendir
    /// </summary>
    public bool Evaluate(decimal transactionAmount, string? mcc = null, string? cardBin = null)
    {
        if (!IsActive)
            return true;

        return RuleType switch
        {
            "MinAmount" => EvaluateMinAmount(transactionAmount),
            "MaxAmount" => EvaluateMaxAmount(transactionAmount),
            "MCC" => EvaluateMcc(mcc),
            "CardBIN" => EvaluateCardBin(cardBin),
            _ => true
        };
    }

    private bool EvaluateMinAmount(decimal amount)
    {
        if (decimal.TryParse(Value, out var minAmount))
        {
            return Operator switch
            {
                ">=" => amount >= minAmount,
                ">" => amount > minAmount,
                _ => amount >= minAmount
            };
        }
        return true;
    }

    private bool EvaluateMaxAmount(decimal amount)
    {
        if (decimal.TryParse(Value, out var maxAmount))
        {
            return Operator switch
            {
                "<=" => amount <= maxAmount,
                "<" => amount < maxAmount,
                _ => amount <= maxAmount
            };
        }
        return true;
    }

    private bool EvaluateMcc(string? mcc)
    {
        if (string.IsNullOrEmpty(mcc))
            return false;

        var allowedMccs = Value.Split(',').Select(x => x.Trim());
        return Operator switch
        {
            "IN" => allowedMccs.Contains(mcc),
            "NOT_IN" => !allowedMccs.Contains(mcc),
            _ => allowedMccs.Contains(mcc)
        };
    }

    private bool EvaluateCardBin(string? cardBin)
    {
        if (string.IsNullOrEmpty(cardBin))
            return false;

        var allowedBins = Value.Split(',').Select(x => x.Trim());
        return Operator switch
        {
            "IN" => allowedBins.Any(bin => cardBin.StartsWith(bin)),
            "NOT_IN" => !allowedBins.Any(bin => cardBin.StartsWith(bin)),
            _ => allowedBins.Any(bin => cardBin.StartsWith(bin))
        };
    }
}