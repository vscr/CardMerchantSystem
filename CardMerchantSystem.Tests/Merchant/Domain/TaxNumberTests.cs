using Merchant.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Merchant.Domain;

public class TaxNumberTests
{
    [Fact]
    public void Create_ValidTaxNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validTaxNumber = "1234567890";

        // Act
        var result = TaxNumber.Create(validTaxNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(validTaxNumber);
    }

    [Fact]
    public void Create_EmptyTaxNumber_ShouldReturnFailure()
    {
        // Arrange
        var emptyTaxNumber = "";

        // Act
        var result = TaxNumber.Create(emptyTaxNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WrongLength_ShouldReturnFailure()
    {
        // Arrange
        var shortTaxNumber = "123456789"; // 9 haneli

        // Act
        var result = TaxNumber.Create(shortTaxNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("10 haneli");
    }

    [Fact]
    public void Create_NonNumeric_ShouldReturnFailure()
    {
        // Arrange
        var invalidTaxNumber = "12345678AB";

        // Act
        var result = TaxNumber.Create(invalidTaxNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Masked_ShouldReturnMaskedValue()
    {
        // Arrange
        var taxNumber = TaxNumber.Create("1234567890").Value!;

        // Act
        var masked = taxNumber.Masked;

        // Assert
        masked.Should().Be("123****890");
    }

    [Fact]
    public void Equals_SameTaxNumber_ShouldBeEqual()
    {
        // Arrange
        var tax1 = TaxNumber.Create("1234567890").Value!;
        var tax2 = TaxNumber.Create("1234567890").Value!;

        // Act & Assert
        tax1.Should().Be(tax2);
    }
}