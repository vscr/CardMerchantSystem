using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.ValueObjects;

/// <summary>
/// Otorizasyon kodu Value Object
/// </summary>
public class AuthorizationCode : ValueObject
{
    public string Value { get; }

    private AuthorizationCode(string value)
    {
        Value = value;
    }

    public static Result<AuthorizationCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<AuthorizationCode>("Otorizasyon kodu boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().ToUpperInvariant();

        if (value.Length != 6)
            return Result.Failure<AuthorizationCode>("Otorizasyon kodu 6 karakter olmalıdır", ErrorCodes.ValidationError);

        return new AuthorizationCode(value);
    }

    /// <summary>
    /// Otomatik otorizasyon kodu üretir
    /// </summary>
    public static AuthorizationCode Generate()
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();
        return new AuthorizationCode(code);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AuthorizationCode code) => code.Value;
}