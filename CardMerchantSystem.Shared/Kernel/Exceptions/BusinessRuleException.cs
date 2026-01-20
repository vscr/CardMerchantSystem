namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// İş kuralı ihlallerinde fırlatılır
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message, string code = "BUSINESS_RULE_VIOLATION")
        : base(message, code)
    {
    }
}