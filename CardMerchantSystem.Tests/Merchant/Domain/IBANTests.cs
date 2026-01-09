using Merchant.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Merchant.Domain;

public class IBANTests
{
    [Fact]
    public void Create_ValidIBAN_ShouldReturnSuccess()
    {
        // Arrange
        var validIban = "TR330006100519786457841326";

        // Act
        var result = IBAN.Create(validIban);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(validIban);
    }

    [Fact]
    public void Create_IBANWithSpaces_ShouldNormalize()
    {
        // Arrange
        var ibanWithSpaces = "TR33 0006 1005 1978 6457 8413 26";

        // Act
        var result = IBAN.Create(ibanWithSpaces);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be("TR330006100519786457841326");
    }

    [Fact]
    public void Create_EmptyIBAN_ShouldReturnFailure()
    {
        // Arrange
        var emptyIban = "";

        // Act
        var result = IBAN.Create(emptyIban);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WrongLength_ShouldReturnFailure()
    {
        // Arrange
        var shortIban = "TR33000610051978645784";

        // Act
        var result = IBAN.Create(shortIban);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("26 karakter");
    }

    [Fact]
    public void Create_NotStartingWithTR_ShouldReturnFailure()
    {
        // Arrange
        var invalidIban = "DE330006100519786457841326";

        // Act
        var result = IBAN.Create(invalidIban);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("TR ile başlamalı");
    }

    [Fact]
    public void Create_InvalidChecksum_ShouldReturnFailure()
    {
        // Arrange
        var invalidIban = "TR330006100519786457841327"; // Yanlış checksum

        // Act
        var result = IBAN.Create(invalidIban);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Formatted_ShouldReturnFormattedValue()
    {
        // Arrange
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        // Act
        var formatted = iban.Formatted;

        // Assert
        formatted.Should().Be("TR33 0006 1005 1978 6457 8413 26");
    }

    [Fact]
    public void Masked_ShouldReturnMaskedValue()
    {
        // Arrange
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        // Act
        var masked = iban.Masked;

        // Assert
        masked.Should().Be("TR33 **** **** **** **** 1326");
    }

    [Fact]
    public void BankCode_ShouldReturnFirst5DigitsAfterTR()
    {
        // Arrange
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        // Act
        var bankCode = iban.BankCode;

        // Assert
        bankCode.Should().Be("00061");
    }

    [Fact]
    public void Equals_SameIBAN_ShouldBeEqual()
    {
        // Arrange
        var iban1 = IBAN.Create("TR330006100519786457841326").Value!;
        var iban2 = IBAN.Create("TR330006100519786457841326").Value!;

        // Act & Assert
        iban1.Should().Be(iban2);
    }
}