namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// Validasyon hatalarında fırlatılır
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Bir veya daha fazla validasyon hatası oluştu", "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}