using CardMerchantSystem.Shared.Kernel;
using System.Numerics;

namespace Merchant.Domain.ValueObjects;

/// <summary>
/// IBAN Value Object
/// Türkiye IBAN formatı: TR + 2 kontrol + 5 banka + 1 rezerv + 16 hesap = 26 karakter
/// </summary>
public class IBAN : ValueObject
{
    public string Value { get; }

    private IBAN(string value)
    {
        Value = value;
    }

    public static Result<IBAN> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<IBAN>("IBAN boş olamaz", ErrorCodes.ValidationError);

        // Boşlukları ve tireleri temizle
        value = value.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .ToUpperInvariant();

        if (value.Length != 26)
            return Result.Failure<IBAN>("Türkiye IBAN'ı 26 karakter olmalıdır", ErrorCodes.ValidationError);

        if (!value.StartsWith("TR"))
            return Result.Failure<IBAN>("Türkiye IBAN'ı TR ile başlamalıdır", ErrorCodes.ValidationError);

        if (!value.Skip(2).All(c => char.IsLetterOrDigit(c)))
            return Result.Failure<IBAN>("IBAN geçersiz karakterler içeriyor", ErrorCodes.ValidationError);

        if (!IsValidIBAN(value))
            return Result.Failure<IBAN>("IBAN geçersiz", ErrorCodes.ValidationError);

        return new IBAN(value);
    }

    private static bool IsValidIBAN(string iban)
    {
        // IBAN doğrulama: İlk 4 karakteri sona taşı, harfleri sayıya çevir, mod 97 = 1
        var rearranged = iban[4..] + iban[..4];

        var numericString = string.Concat(rearranged.Select(c =>
            char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        var number = BigInteger.Parse(numericString);
        return number % 97 == 1;
    }

    /// <summary>
    /// Formatlanmış IBAN (4'lü gruplar)
    /// </summary>
    public string Formatted => string.Join(" ",
        Enumerable.Range(0, (Value.Length + 3) / 4)
            .Select(i => Value.Substring(i * 4, Math.Min(4, Value.Length - i * 4))));

    /// <summary>
    /// Maskelenmiş IBAN
    /// </summary>
    public string Masked => $"{Value[..4]} **** **** **** **** {Value[^4..]}";

    /// <summary>
    /// Banka kodu (5-9. karakterler)
    /// </summary>
    public string BankCode => Value.Substring(4, 5);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Formatted;

    public static implicit operator string(IBAN iban) => iban.Value;
}