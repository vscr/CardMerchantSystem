using Card.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Card.Domain;

public class TCKNTests
{
    [Fact]
    public void Create_ValidTCKN_ShouldReturnSuccess()
    {
        // Arrange
        var validTckn = "10000000146";

        // Act
        var result = TCKN.Create(validTckn);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(validTckn);
    }

    [Fact]
    public void Create_InvalidChecksum_ShouldReturnFailure()
    {
        // Arrange
        var invalidTckn = "10000000147"; // Yanlış checksum

        // Act
        var result = TCKN.Create(invalidTckn);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("geçersiz");
    }

    [Fact]
    public void Create_EmptyTCKN_ShouldReturnFailure()
    {
        // Arrange
        var emptyTckn = "";

        // Act
        var result = TCKN.Create(emptyTckn);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_TCKNStartsWithZero_ShouldReturnFailure()
    {
        // Arrange
        var invalidTckn = "01234567890";

        // Act
        var result = TCKN.Create(invalidTckn);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("0 ile başlayamaz");
    }

    [Fact]
    public void Create_TCKNWrongLength_ShouldReturnFailure()
    {
        // Arrange
        var shortTckn = "1234567890"; // 10 haneli

        // Act
        var result = TCKN.Create(shortTckn);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("11 haneli");
    }

    [Fact]
    public void Masked_ShouldReturnMaskedValue()
    {
        // Arrange
        var tckn = TCKN.Create("10000000146").Value!;

        // Act
        var masked = tckn.Masked;

        // Assert
        masked.Should().Be("100******46");
    }

    [Fact]
    public void Equals_SameTCKN_ShouldBeEqual()
    {
        // Arrange
        var tckn1 = TCKN.Create("10000000146").Value!;
        var tckn2 = TCKN.Create("10000000146").Value!;

        // Act & Assert
        tckn1.Should().Be(tckn2);
    }
}