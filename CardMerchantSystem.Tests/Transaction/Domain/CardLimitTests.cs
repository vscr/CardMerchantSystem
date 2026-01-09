using Transaction.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Transaction.Domain;

public class CardLimitTests
{
    [Fact]
    public void Create_ShouldSetAllProperties()
    {
        // Arrange & Act
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 2000m, 5000m, "TRY");

        // Assert
        limit.CardNumber.Should().Be("4539 **** **** 1234");
        limit.DailyLimit.Should().Be(10000m);
        limit.MonthlyLimit.Should().Be(50000m);
        limit.DailyUsed.Should().Be(2000m);
        limit.MonthlyUsed.Should().Be(5000m);
        limit.Currency.Should().Be("TRY");
    }

    [Fact]
    public void RemainingDailyLimit_ShouldCalculateCorrectly()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 3000m, 5000m);

        // Act
        var remaining = limit.RemainingDailyLimit;

        // Assert
        remaining.Should().Be(7000m);
    }

    [Fact]
    public void RemainingMonthlyLimit_ShouldCalculateCorrectly()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 3000m, 15000m);

        // Act
        var remaining = limit.RemainingMonthlyLimit;

        // Assert
        remaining.Should().Be(35000m);
    }

    [Fact]
    public void RemainingLimit_WhenOverused_ShouldReturnZero()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 12000m, 55000m);

        // Act & Assert
        limit.RemainingDailyLimit.Should().Be(0);
        limit.RemainingMonthlyLimit.Should().Be(0);
    }

    [Fact]
    public void CanProcess_WithinBothLimits_ShouldReturnTrue()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 2000m, 5000m);

        // Act
        var canProcess = limit.CanProcess(5000m);

        // Assert
        canProcess.Should().BeTrue();
    }

    [Fact]
    public void CanProcess_ExceedsDailyLimit_ShouldReturnFalse()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 8000m, 10000m);

        // Act
        var canProcess = limit.CanProcess(5000m); // 8000 + 5000 > 10000

        // Assert
        canProcess.Should().BeFalse();
    }

    [Fact]
    public void CanProcess_ExceedsMonthlyLimit_ShouldReturnFalse()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 2000m, 48000m);

        // Act
        var canProcess = limit.CanProcess(5000m); // 48000 + 5000 > 50000

        // Assert
        canProcess.Should().BeFalse();
    }

    [Fact]
    public void UseLimit_ShouldIncreaseUsedAmounts()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 2000m, 5000m);

        // Act
        var newLimit = limit.UseLimit(1500m);

        // Assert
        newLimit.DailyUsed.Should().Be(3500m);
        newLimit.MonthlyUsed.Should().Be(6500m);
    }

    [Fact]
    public void ReleaseLimit_ShouldDecreaseUsedAmounts()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 5000m, 10000m);

        // Act
        var newLimit = limit.ReleaseLimit(2000m);

        // Assert
        newLimit.DailyUsed.Should().Be(3000m);
        newLimit.MonthlyUsed.Should().Be(8000m);
    }

    [Fact]
    public void ReleaseLimit_ShouldNotGoBelowZero()
    {
        // Arrange
        var limit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 1000m, 2000m);

        // Act
        var newLimit = limit.ReleaseLimit(5000m);

        // Assert
        newLimit.DailyUsed.Should().Be(0);
        newLimit.MonthlyUsed.Should().Be(0);
    }

    [Fact]
    public void UseLimit_ShouldBeImmutable()
    {
        // Arrange
        var originalLimit = CardLimit.Create("4539 **** **** 1234", 10000m, 50000m, 2000m, 5000m);

        // Act
        var newLimit = originalLimit.UseLimit(1000m);

        // Assert
        originalLimit.DailyUsed.Should().Be(2000m); // Original unchanged
        newLimit.DailyUsed.Should().Be(3000m);
    }
}