using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.ValueObjects;

/// <summary>
/// Vergi Numarası Value Object
/// 10 haneli vergi kimlik numarası
/// </summary>
public class TaxNumber : ValueObject
{
    public string Value { get; }

    private TaxNumber(string value)
    {
        Value = value;
    }

    public static Result<TaxNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<TaxNumber>("Vergi numarası boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().Replace(" ", "");

        if (value.Length != 10)
            return Result.Failure<TaxNumber>("Vergi numarası 10 haneli olmalıdır", ErrorCodes.ValidationError);

        if (!value.All(char.IsDigit))
            return Result.Failure<TaxNumber>("Vergi numarası sadece rakamlardan oluşmalıdır", ErrorCodes.ValidationError);

        if (!IsValidChecksum(value))
            return Result.Failure<TaxNumber>("Vergi numarası geçersiz", ErrorCodes.ValidationError);

        return new TaxNumber(value);
    }

    private static bool IsValidChecksum(string vkn)
    {
        var digits = vkn.Select(c => int.Parse(c.ToString())).ToArray();

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int tmp = (digits[i] + (9 - i)) % 10;
            sum += (tmp * (int)Math.Pow(2, 9 - i)) % 9;
            if (tmp != 0 && (tmp * (int)Math.Pow(2, 9 - i)) % 9 == 0)
                sum += 9;
        }

        return (10 - (sum % 10)) % 10 == digits[9];
    }

    public string Masked => $"{Value[..3]}****{Value[^3..]}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TaxNumber taxNumber) => taxNumber.Value;
}