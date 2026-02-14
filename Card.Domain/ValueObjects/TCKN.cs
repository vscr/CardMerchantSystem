using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.ValueObjects;

/// <summary>
/// TC Kimlik Numarası Value Object.
/// 11 haneli, algoritma ile doğrulanabilen kimlik numarası.
/// </summary>
public class TCKN : ValueObject
{
    public string Value { get; }

    private TCKN(string value)
    {
        Value = value;
    }

    public static Result<TCKN> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<TCKN>("TCKN boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().Replace(" ", "");

        if (value.Length != 11)
            return Result.Failure<TCKN>("TCKN 11 haneli olmalıdır", ErrorCodes.ValidationError);

        if (!value.All(char.IsDigit))
            return Result.Failure<TCKN>("TCKN sadece rakamlardan oluşmalıdır", ErrorCodes.ValidationError);

        //if (value[0] == '0')
        //    return Result.Failure<TCKN>("TCKN 0 ile başlayamaz", ErrorCodes.ValidationError);

        //if (!IsValidChecksum(value))
        //    return Result.Failure<TCKN>("TCKN geçersiz", ErrorCodes.ValidationError);

        return new TCKN(value);
    }

    private static bool IsValidChecksum(string tckn)
    {
        var digits = tckn.Select(c => int.Parse(c.ToString())).ToArray();

        // 10. hane kontrolü
        var oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        var evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        var digit10 = ((oddSum * 7) - evenSum) % 10;

        if (digit10 < 0) digit10 += 10;
        if (digits[9] != digit10)
            return false;

        // 11. hane kontrolü
        var sum = digits.Take(10).Sum();
        var digit11 = sum % 10;

        return digits[10] == digit11;
    }

    /// <summary>
    /// Maskelenmiş TCKN (ilk 3 ve son 2 hane görünür)
    /// </summary>
    public string Masked => $"{Value[..3]}******{Value[^2..]}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TCKN tckn) => tckn.Value;
}