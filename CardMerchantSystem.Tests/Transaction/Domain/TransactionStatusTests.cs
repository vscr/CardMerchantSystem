using Transaction.Domain.Enums;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Transaction.Domain;

public class TransactionStatusTests
{
    [Fact]
    public void Approved_IsSuccessful_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Approved;

        // Act & Assert
        status.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Settled_IsSuccessful_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Settled;

        // Act & Assert
        status.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Declined_IsSuccessful_ShouldBeFalse()
    {
        // Arrange
        var status = TransactionStatus.Declined;

        // Act & Assert
        status.IsSuccessful.Should().BeFalse();
    }

    [Fact]
    public void Pending_IsSuccessful_ShouldBeFalse()
    {
        // Arrange
        var status = TransactionStatus.Pending;

        // Act & Assert
        status.IsSuccessful.Should().BeFalse();
    }

    [Fact]
    public void Declined_IsFinal_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Declined;

        // Act & Assert
        status.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Settled_IsFinal_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Settled;

        // Act & Assert
        status.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Reversed_IsFinal_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Reversed;

        // Act & Assert
        status.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Error_IsFinal_ShouldBeTrue()
    {
        // Arrange
        var status = TransactionStatus.Error;

        // Act & Assert
        status.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Pending_IsFinal_ShouldBeFalse()
    {
        // Arrange
        var status = TransactionStatus.Pending;

        // Act & Assert
        status.IsFinal.Should().BeFalse();
    }

    [Fact]
    public void Approved_IsFinal_ShouldBeFalse()
    {
        // Arrange
        var status = TransactionStatus.Approved;

        // Act & Assert
        status.IsFinal.Should().BeFalse();
    }

    [Fact]
    public void FromId_ValidId_ShouldReturnStatus()
    {
        // Act
        var status = TransactionStatus.FromId<TransactionStatus>(2);

        // Assert
        status.Should().Be(TransactionStatus.Approved);
    }

    [Fact]
    public void FromName_ValidName_ShouldReturnStatus()
    {
        // Act
        var status = TransactionStatus.FromName<TransactionStatus>("Declined");

        // Assert
        status.Should().Be(TransactionStatus.Declined);
    }
}