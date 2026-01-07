using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.ValueObjects;

/// <summary>
/// Para Value Object.
/// Tutar ve para birimi birlikte tutulur.
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Result<Money> Create(decimal amount, string currency = "TRY")
    {
        if (amount < 0)
            return Result.Failure<Money>("Tutar negatif olamaz", ErrorCodes.ValidationError);

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Money>("Para birimi belirtilmeli", ErrorCodes.ValidationError);

        if (currency.Length != 3)
            return Result.Failure<Money>("Para birimi 3 karakterli olmalı (ISO 4217)", ErrorCodes.ValidationError);

        return new Money(amount, currency);
    }

    public static Money Zero(string currency = "TRY") => new(0, currency);

    public static Money TRY(decimal amount) => new(amount, "TRY");

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Farklı para birimleri toplanamaz: {left.Currency} ve {right.Currency}");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Farklı para birimleri çıkarılamaz: {left.Currency} ve {right.Currency}");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Farklı para birimleri karşılaştırılamaz");
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Farklı para birimleri karşılaştırılamaz");
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right) => left > right || left == right;
    public static bool operator <=(Money left, Money right) => left < right || left == right;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}