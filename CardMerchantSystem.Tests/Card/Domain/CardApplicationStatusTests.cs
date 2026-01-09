using Card.Domain.Enums;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Card.Domain;

public class CardApplicationStatusTests
{
    [Fact]
    public void Pending_CanTransitionTo_UnderReview()
    {
        // Arrange
        var status = CardApplicationStatus.Pending;

        // Act
        var canTransition = status.CanTransitionTo(CardApplicationStatus.UnderReview);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Pending_CannotTransitionTo_Approved()
    {
        // Arrange
        var status = CardApplicationStatus.Pending;

        // Act
        var canTransition = status.CanTransitionTo(CardApplicationStatus.Approved);

        // Assert
        canTransition.Should().BeFalse();
    }

    [Fact]
    public void UnderReview_CanTransitionTo_Approved()
    {
        // Arrange
        var status = CardApplicationStatus.UnderReview;

        // Act
        var canTransition = status.CanTransitionTo(CardApplicationStatus.Approved);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void UnderReview_CanTransitionTo_Rejected()
    {
        // Arrange
        var status = CardApplicationStatus.UnderReview;

        // Act
        var canTransition = status.CanTransitionTo(CardApplicationStatus.Rejected);

        // Assert
        canTransition.Should().BeTrue();
    }

    [Fact]
    public void Delivered_IsFinalState()
    {
        // Arrange
        var status = CardApplicationStatus.Delivered;

        // Act & Assert
        status.IsFinalState.Should().BeTrue();
    }

    [Fact]
    public void Rejected_IsFinalState()
    {
        // Arrange
        var status = CardApplicationStatus.Rejected;

        // Act & Assert
        status.IsFinalState.Should().BeTrue();
    }

    [Fact]
    public void Pending_IsNotFinalState()
    {
        // Arrange
        var status = CardApplicationStatus.Pending;

        // Act & Assert
        status.IsFinalState.Should().BeFalse();
    }

    [Fact]
    public void Pending_IsCancellable()
    {
        // Arrange
        var status = CardApplicationStatus.Pending;

        // Act & Assert
        status.IsCancellable.Should().BeTrue();
    }

    [Fact]
    public void Delivered_IsNotCancellable()
    {
        // Arrange
        var status = CardApplicationStatus.Delivered;

        // Act & Assert
        status.IsCancellable.Should().BeFalse();
    }

    [Fact]
    public void FromId_ValidId_ShouldReturnStatus()
    {
        // Act
        var status = CardApplicationStatus.FromId<CardApplicationStatus>(1);

        // Assert
        status.Should().Be(CardApplicationStatus.Pending);
    }

    [Fact]
    public void FromName_ValidName_ShouldReturnStatus()
    {
        // Act
        var status = CardApplicationStatus.FromName<CardApplicationStatus>("Approved");

        // Assert
        status.Should().Be(CardApplicationStatus.Approved);
    }
}