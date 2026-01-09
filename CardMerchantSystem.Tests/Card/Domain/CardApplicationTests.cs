using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Card.Domain;

public class CardApplicationTests
{
    private CardApplication CreateValidApplication()
    {
        var tckn = TCKN.Create("10000000146").Value!;
        var address = Address.Create("Test Sokak", "Konak", "İzmir", "35000", "Türkiye").Value!;

        var result = CardApplication.Create(
            tckn,
            "Volkan",
            "Test",
            "5551234567",
            "volkan@test.com",
            address,
            CardType.Credit);

        return result.Value!;
    }

    [Fact]
    public void Create_ValidData_ShouldReturnSuccess()
    {
        // Arrange
        var tckn = TCKN.Create("10000000146").Value!;
        var address = Address.Create("Test Sokak", "Konak", "İzmir", "35000", "Türkiye").Value!;

        // Act
        var result = CardApplication.Create(
            tckn,
            "Volkan",
            "Test",
            "5551234567",
            "volkan@test.com",
            address,
            CardType.Credit);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(CardApplicationStatus.Pending);
        result.Value.CustomerFullName.Should().Be("Volkan Test");
    }

    [Fact]
    public void Create_WithCustomLimits_ShouldUseCustomLimits()
    {
        // Arrange
        var tckn = TCKN.Create("10000000146").Value!;
        var address = Address.Create("Test Sokak", "Konak", "İzmir", "35000", "Türkiye").Value!;
        var dailyLimit = Money.TRY(5000);
        var monthlyLimit = Money.TRY(20000);

        // Act
        var result = CardApplication.Create(
            tckn,
            "Volkan",
            "Test",
            "5551234567",
            "volkan@test.com",
            address,
            CardType.Credit,
            dailyLimit,
            monthlyLimit);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DailyLimit.Amount.Should().Be(5000);
        result.Value.MonthlyLimit.Amount.Should().Be(20000);
    }

    [Fact]
    public void Create_ShouldAddDomainEvent()
    {
        // Arrange & Act
        var application = CreateValidApplication();

        // Assert
        application.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Create_ShouldAddStatusHistory()
    {
        // Arrange & Act
        var application = CreateValidApplication();

        // Assert
        application.StatusHistory.Should().HaveCount(1);
    }

    [Fact]
    public void StartReview_FromPending_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();

        // Act
        var result = application.StartReview("admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.UnderReview);
    }

    [Fact]
    public void Approve_FromUnderReview_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");

        // Act
        var result = application.Approve("admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.Approved);
        application.ApprovedBy.Should().Be("admin");
        application.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_FromPending_ShouldFail()
    {
        // Arrange
        var application = CreateValidApplication();

        // Act
        var result = application.Approve("admin");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Reject_FromUnderReview_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");

        // Act
        var result = application.Reject("Yetersiz gelir", "admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.Rejected);
        application.RejectionReason.Should().Be("Yetersiz gelir");
    }

    [Fact]
    public void Reject_WithoutReason_ShouldFail()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");

        // Act
        var result = application.Reject("", "admin");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RequestCardPrint_FromApproved_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");
        application.Approve("admin");

        // Act
        var result = application.RequestCardPrint(PrintVendor.Bilesim, "BATCH-001", "operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.CardRequested);
        application.PrintVendor.Should().Be(PrintVendor.Bilesim);
        application.PrintBatchId.Should().Be("BATCH-001");
    }

    [Fact]
    public void MarkAsPrinted_ShouldSetCardNumber()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");
        application.Approve("admin");
        application.RequestCardPrint(PrintVendor.Bilesim, "BATCH-001", "operator");

        // Act
        var result = application.MarkAsPrinted("ENC_123", "4539 **** **** 1234", "operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.CardPrinted);
        application.CardNumberMasked.Should().Be("4539 **** **** 1234");
        application.PrintedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_FromPending_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();

        // Act
        var result = application.Cancel("Müşteri talebi", "operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.Status.Should().Be(CardApplicationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromDelivered_ShouldFail()
    {
        // Arrange
        var application = CreateValidApplication();
        application.StartReview("admin");
        application.Approve("admin");
        application.RequestCardPrint(PrintVendor.Bilesim, "BATCH-001", "operator");
        application.MarkAsPrinted("ENC_123", "4539 **** **** 1234", "operator");
        application.MarkAsReadyForDelivery("operator");
        application.StartDelivery("TRACK-001", "operator");
        application.MarkAsDelivered("operator");

        // Act
        var result = application.Cancel("Test", "operator");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateLimits_ValidLimits_ShouldSucceed()
    {
        // Arrange
        var application = CreateValidApplication();
        var newDailyLimit = Money.TRY(10000);
        var newMonthlyLimit = Money.TRY(50000);

        // Act
        var result = application.UpdateLimits(newDailyLimit, newMonthlyLimit, "admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        application.DailyLimit.Amount.Should().Be(10000);
        application.MonthlyLimit.Amount.Should().Be(50000);
    }

    [Fact]
    public void UpdateLimits_DailyGreaterThanMonthly_ShouldFail()
    {
        // Arrange
        var application = CreateValidApplication();
        var newDailyLimit = Money.TRY(30000);
        var newMonthlyLimit = Money.TRY(20000);

        // Act
        var result = application.UpdateLimits(newDailyLimit, newMonthlyLimit, "admin");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FullLifecycle_ShouldTrackAllStatusChanges()
    {
        // Arrange
        var application = CreateValidApplication();

        // Act - Full lifecycle
        application.StartReview("admin");
        application.Approve("admin");
        application.RequestCardPrint(PrintVendor.Bilesim, "BATCH-001", "operator");
        application.MarkAsPrinted("ENC_123", "4539 **** **** 1234", "operator");
        application.MarkAsReadyForDelivery("operator");
        application.StartDelivery("TRACK-001", "operator");
        application.MarkAsDelivered("operator");

        // Assert
        application.Status.Should().Be(CardApplicationStatus.Delivered);
        application.StatusHistory.Should().HaveCount(8); // Pending + 7 transitions
    }
}