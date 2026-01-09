using Merchant.Domain.Enums;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Merchant.Domain;

public class MerchantStatusTests
{
    [Fact]
    public void Pending_CanTransitionTo_UnderReview()
    {
        // Arrange
        var status = MerchantStatus.Pending;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.UnderReview);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Pending_CanTransitionTo_Rejected()
    {
        // Arrange
        var status = MerchantStatus.Pending;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Rejected);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Pending_CannotTransitionTo_Active()
    {
        // Arrange
        var status = MerchantStatus.Pending;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Active);

        // Assert
        canTransition.Should().BeFalse();
    }

    [Fact]
    public void UnderReview_CanTransitionTo_Approved()
    {
        // Arrange
        var status = MerchantStatus.UnderReview;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Approved);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Approved_CanTransitionTo_Active()
    {
        // Arrange
        var status = MerchantStatus.Approved;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Active);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Active_CanTransitionTo_Suspended()
    {
        // Arrange
        var status = MerchantStatus.Active;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Suspended);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Active_CanTransitionTo_Closed()
    {
        // Arrange
        var status = MerchantStatus.Active;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Closed);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Suspended_CanTransitionTo_Active()
    {
        // Arrange
        var status = MerchantStatus.Suspended;

        // Act
        var canTransition = status.CanTransitionTo(MerchantStatus.Active);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Closed_CannotTransitionToAnything()
    {
        // Arrange
        var status = MerchantStatus.Closed;

        // Act & Assert
        status.CanTransitionTo(MerchantStatus.Active).Should().BeFalse();
        status.CanTransitionTo(MerchantStatus.Pending).Should().BeFalse();
        status.AllowedTransitions.Should().BeEmpty();
    }

    [Fact]
    public void Active_CanProcessTransactions()
    {
        // Arrange
        var status = MerchantStatus.Active;

        // Act & Assert
        status.CanProcessTransactions.Should().BeTrue();
    }

    [Fact]
    public void Pending_CannotProcessTransactions()
    {
        // Arrange
        var status = MerchantStatus.Pending;

        // Act & Assert
        status.CanProcessTransactions.Should().BeFalse();
    }

    [Fact]
    public void FromId_ValidId_ShouldReturnStatus()
    {
        // Act
        var status = MerchantStatus.FromId<MerchantStatus>(4);

        // Assert
        status.Should().Be(MerchantStatus.Active);
    }

    [Fact]
    public void FromName_ValidName_ShouldReturnStatus()
    {
        // Act
        var status = MerchantStatus.FromName<MerchantStatus>("Suspended");

        // Assert
        status.Should().Be(MerchantStatus.Suspended);
    }
}