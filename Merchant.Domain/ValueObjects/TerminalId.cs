using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.ValueObjects;

/// <summary>
/// Terminal ID Value Object
/// 8 haneli benzersiz terminal tanımlayıcı
/// </summary>
public class TerminalId : ValueObject
{
    public string Value { get; }

    private TerminalId(string value)
    {
        Value = value;
    }

    public static Result<TerminalId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<TerminalId>("Terminal ID boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().ToUpperInvariant();

        if (value.Length != 8)
            return Result.Failure<TerminalId>("Terminal ID 8 karakter olmalıdır", ErrorCodes.ValidationError);

        if (!value.All(c => char.IsLetterOrDigit(c)))
            return Result.Failure<TerminalId>("Terminal ID sadece harf ve rakamlardan oluşmalıdır", ErrorCodes.ValidationError);

        return new TerminalId(value);
    }

    /// <summary>
    /// Otomatik terminal ID üretir
    /// Format: T + 7 haneli sayı
    /// </summary>
    public static TerminalId Generate()
    {
        var random = new Random().Next(1000000, 9999999);
        var id = $"T{random}";
        return new TerminalId(id);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TerminalId terminalId) => terminalId.Value;
}