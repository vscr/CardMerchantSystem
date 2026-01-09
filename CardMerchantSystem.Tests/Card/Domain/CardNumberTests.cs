using Card.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Card.Domain;

public class CardNumberTests
{
    [Fact]
    public void Create_ValidCardNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validCardNumber = "4539578763621486"; // Luhn geçerli

        // Act
        var result = CardNumber.Create(validCardNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(validCardNumber);
    }

    [Fact]
    public void Create_InvalidLuhn_ShouldReturnFailure()
    {
        // Arrange
        var invalidCardNumber = "4539578763621487"; // Luhn geçersiz

        // Act
        var result = CardNumber.Create(invalidCardNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("geçersiz");
    }

    [Fact]
    public void Create_EmptyCardNumber_ShouldReturnFailure()
    {
        // Arrange
        var emptyCardNumber = "";

        // Act
        var result = CardNumber.Create(emptyCardNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WrongLength_ShouldReturnFailure()
    {
        // Arrange
        var shortCardNumber = "453957876362"; // 12 haneli

        // Act
        var result = CardNumber.Create(shortCardNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("16 haneli");
    }

    [Fact]
    public void Masked_ShouldReturnMaskedValue()
    {
        // Arrange
        var cardNumber = CardNumber.Create("4539578763621486").Value!;

        // Act
        var masked = cardNumber.Masked;

        // Assert
        masked.Should().Be("4539 **** **** 1486");
    }

    [Fact]
    public void BIN_ShouldReturnFirst6Digits()
    {
        // Arrange
        var cardNumber = CardNumber.Create("4539578763621486").Value!;

        // Act
        var bin = cardNumber.BIN;

        // Assert
        bin.Should().Be("453957");
    }

    [Fact]
    public void LastFourDigits_ShouldReturnLast4Digits()
    {
        // Arrange
        var cardNumber = CardNumber.Create("4539578763621486").Value!;

        // Act
        var lastFour = cardNumber.LastFourDigits;

        // Assert
        lastFour.Should().Be("1486");
    }

    [Fact]
    public void GenerateTest_ShouldReturnValidCardNumber()
    {
        // Act
        var cardNumber = CardNumber.GenerateTest();

        // Assert
        cardNumber.Should().NotBeNull();
        cardNumber.Value.Should().HaveLength(16);
    }

    [Fact]
    public void Equals_SameCardNumber_ShouldBeEqual()
    {
        // Arrange
        var card1 = CardNumber.Create("4539578763621486").Value!;
        var card2 = CardNumber.Create("4539578763621486").Value!;

        // Act & Assert
        card1.Should().Be(card2);
    }
}