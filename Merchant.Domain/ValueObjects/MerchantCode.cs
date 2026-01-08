using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.ValueObjects;

/// <summary>
/// Üye İşyeri Kodu (Merchant ID)
/// Benzersiz işyeri tanımlayıcı
/// </summary>
public class MerchantCode : ValueObject
{
    public string Value { get; }

    private MerchantCode(string value)
    {
        Value = value;
    }

    public static Result<MerchantCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<MerchantCode>("Üye işyeri kodu boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().ToUpperInvariant();

        if (value.Length < 8 || value.Length > 15)
            return Result.Failure<MerchantCode>("Üye işyeri kodu 8-15 karakter olmalıdır", ErrorCodes.ValidationError);

        if (!value.All(c => char.IsLetterOrDigit(c)))
            return Result.Failure<MerchantCode>("Üye işyeri kodu sadece harf ve rakamlardan oluşmalıdır", ErrorCodes.ValidationError);

        return new MerchantCode(value);
    }

    /// <summary>
    /// Otomatik üye işyeri kodu üretir
    /// Format: MRC + Timestamp + Random
    /// </summary>
    public static MerchantCode Generate()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        var code = $"MRC{timestamp}{random}";
        return new MerchantCode(code);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(MerchantCode code) => code.Value;
}