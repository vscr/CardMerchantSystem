using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.ValueObjects;

/// <summary>
/// Kart Numarası Value Object.
/// 16 haneli, Luhn algoritması ile doğrulanabilen kart numarası.
/// PCI-DSS uyumluluğu için maskeleme desteği.
/// </summary>
public class CardNumber : ValueObject
{
    public string Value { get; }

    private CardNumber(string value)
    {
        Value = value;
    }

    public static Result<CardNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<CardNumber>("Kart numarası boş olamaz", ErrorCodes.ValidationError);

        value = value.Trim().Replace(" ", "").Replace("-", "");

        if (value.Length != 16)
            return Result.Failure<CardNumber>("Kart numarası 16 haneli olmalıdır", ErrorCodes.ValidationError);

        if (!value.All(char.IsDigit))
            return Result.Failure<CardNumber>("Kart numarası sadece rakamlardan oluşmalıdır", ErrorCodes.ValidationError);

        if (!IsValidLuhn(value))
            return Result.Failure<CardNumber>("Kart numarası geçersiz (Luhn kontrolü başarısız)", ErrorCodes.ValidationError);

        return new CardNumber(value);
    }

    /// <summary>
    /// BIN (Bank Identification Number) - ilk 6 hane
    /// </summary>
    public string BIN => Value[..6];

    /// <summary>
    /// Son 4 hane (müşteriye gösterilebilir)
    /// </summary>
    public string LastFourDigits => Value[^4..];

    /// <summary>
    /// Maskelenmiş kart numarası (PCI-DSS uyumlu)
    /// </summary>
    public string Masked => $"{Value[..4]} **** **** {Value[^4..]}";

    /// <summary>
    /// Luhn algoritması ile doğrulama
    /// </summary>
    private static bool IsValidLuhn(string number)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(number[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// Test için geçerli kart numarası üretir
    /// </summary>
    public static CardNumber GenerateTest(string bin = "454360")
    {
        var random = new Random();
        var number = bin;

        while (number.Length < 15)
        {
            number += random.Next(0, 10).ToString();
        }

        // Luhn check digit hesapla
        int sum = 0;
        bool alternate = true;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(number[i].ToString());
            if (alternate)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
            alternate = !alternate;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        number += checkDigit.ToString();

        return new CardNumber(number);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Masked;
}