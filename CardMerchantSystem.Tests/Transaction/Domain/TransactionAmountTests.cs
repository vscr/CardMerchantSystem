using Transaction.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Transaction.Domain;

public class TransactionAmountTests
{
    [Fact]
    public void Create_ValidAmount_ShouldReturnSuccess()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(1500.50m, "TRY");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(1500.50m);
        result.Value.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Create_ZeroAmount_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(0, "TRY");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("sıfırdan büyük");
    }

    [Fact]
    public void Create_NegativeAmount_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(-100m, "TRY");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_EmptyCurrency_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(100m, "");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_InvalidCurrencyLength_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(100m, "TRYY");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("3 karakterli");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrency()
    {
        // Arrange & Act
        var result = TransactionAmount.Create(100m, "try");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        // Arrange & Act
        var zero = TransactionAmount.Zero("TRY");

        // Assert
        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("TRY");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var amount = TransactionAmount.Create(1500.50m, "TRY").Value!;

        // Act
        var result = amount.ToString();

        // Assert
        result.Should().Contain("1");
        result.Should().Contain("TRY");
    }

    [Fact]
    public void Equals_SameAmount_ShouldBeEqual()
    {
        // Arrange
        var amount1 = TransactionAmount.Create(100m, "TRY").Value!;
        var amount2 = TransactionAmount.Create(100m, "TRY").Value!;

        // Act & Assert
        amount1.Should().Be(amount2);
    }

    [Fact]
    public void Equals_DifferentCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var amount1 = TransactionAmount.Create(100m, "TRY").Value!;
        var amount2 = TransactionAmount.Create(100m, "USD").Value!;

        // Act & Assert
        amount1.Should().NotBe(amount2);
    }
}