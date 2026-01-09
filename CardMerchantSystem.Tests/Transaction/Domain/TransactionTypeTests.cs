using Transaction.Domain.Enums;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Transaction.Domain;

public class TransactionTypeTests
{
    [Fact]
    public void Sale_DecreasesLimit_ShouldBeTrue()
    {
        // Arrange
        var type = TransactionType.Sale;

        // Act & Assert
        type.DecreasesLimit.Should().BeTrue();
    }

    [Fact]
    public void PreAuth_DecreasesLimit_ShouldBeTrue()
    {
        // Arrange
        var type = TransactionType.PreAuth;

        // Act & Assert
        type.DecreasesLimit.Should().BeTrue();
    }

    [Fact]
    public void CashAdvance_DecreasesLimit_ShouldBeTrue()
    {
        // Arrange
        var type = TransactionType.CashAdvance;

        // Act & Assert
        type.DecreasesLimit.Should().BeTrue();
    }

    [Fact]
    public void Refund_DecreasesLimit_ShouldBeFalse()
    {
        // Arrange
        var type = TransactionType.Refund;

        // Act & Assert
        type.DecreasesLimit.Should().BeFalse();
    }

    [Fact]
    public void Cancel_DecreasesLimit_ShouldBeFalse()
    {
        // Arrange
        var type = TransactionType.Cancel;

        // Act & Assert
        type.DecreasesLimit.Should().BeFalse();
    }

    [Fact]
    public void Refund_IncreasesLimit_ShouldBeTrue()
    {
        // Arrange
        var type = TransactionType.Refund;

        // Act & Assert
        type.IncreasesLimit.Should().BeTrue();
    }

    [Fact]
    public void Cancel_IncreasesLimit_ShouldBeTrue()
    {
        // Arrange
        var type = TransactionType.Cancel;

        // Act & Assert
        type.IncreasesLimit.Should().BeTrue();
    }

    [Fact]
    public void Sale_IncreasesLimit_ShouldBeFalse()
    {
        // Arrange
        var type = TransactionType.Sale;

        // Act & Assert
        type.IncreasesLimit.Should().BeFalse();
    }

    [Fact]
    public void FromId_ValidId_ShouldReturnType()
    {
        // Act
        var type = TransactionType.FromId<TransactionType>(1);

        // Assert
        type.Should().Be(TransactionType.Sale);
    }

    [Fact]
    public void FromName_ValidName_ShouldReturnType()
    {
        // Act
        var type = TransactionType.FromName<TransactionType>("Refund");

        // Assert
        type.Should().Be(TransactionType.Refund);
    }
}