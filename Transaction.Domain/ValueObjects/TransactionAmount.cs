using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.ValueObjects;

/// <summary>
/// İşlem tutarı Value Object
/// </summary>
public class TransactionAmount : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private TransactionAmount(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Result<TransactionAmount> Create(decimal amount, string currency = "TRY")
    {
        if (amount <= 0)
            return Result.Failure<TransactionAmount>("İşlem tutarı sıfırdan büyük olmalıdır", ErrorCodes.ValidationError);

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<TransactionAmount>("Para birimi belirtilmeli", ErrorCodes.ValidationError);

        if (currency.Length != 3)
            return Result.Failure<TransactionAmount>("Para birimi 3 karakterli olmalı (ISO 4217)", ErrorCodes.ValidationError);

        return new TransactionAmount(amount, currency);
    }

    public static TransactionAmount Zero(string currency = "TRY") => new(0, currency);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}