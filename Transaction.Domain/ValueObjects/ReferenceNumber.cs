using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.ValueObjects;

/// <summary>
/// Referans numarası Value Object (RRN - Retrieval Reference Number)
/// </summary>
public class ReferenceNumber : ValueObject
{
    public string Value { get; }

    private ReferenceNumber(string value)
    {
        Value = value;
    }

    public static Result<ReferenceNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<ReferenceNumber>("Referans numarası boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim();

        if (value.Length != 12)
            return Result.Failure<ReferenceNumber>("Referans numarası 12 karakter olmalıdır", ErrorCodes.ValidationError);

        return new ReferenceNumber(value);
    }

    /// <summary>
    /// Otomatik referans numarası üretir
    /// Format: YYMMDD + 6 haneli sayı
    /// </summary>
    public static ReferenceNumber Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyMMdd");
        var random = new Random().Next(100000, 999999);
        var rrn = $"{datePart}{random}";
        return new ReferenceNumber(rrn);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ReferenceNumber rrn) => rrn.Value;
}