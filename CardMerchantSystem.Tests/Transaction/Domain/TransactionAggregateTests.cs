using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Transaction.Domain;

public class TransactionAggregateTests
{
    private TransactionAggregate CreateValidTransaction()
    {
        var amount = TransactionAmount.Create(1500m, "TRY").Value!;

        var result = TransactionAggregate.Create(
            TransactionType.Sale,
            amount,
            "4539 **** **** 1234",
            "ENC_4539_1234_XYZ",
            Guid.NewGuid(),
            "MRC20240101001",
            Guid.NewGuid(),
            "T1234567");

        return result.Value!;
    }

    [Fact]
    public void Create_ValidData_ShouldReturnSuccess()
    {
        // Arrange
        var amount = TransactionAmount.Create(1500m, "TRY").Value!;

        // Act
        var result = TransactionAggregate.Create(
            TransactionType.Sale,
            amount,
            "4539 **** **** 1234",
            "ENC_4539_1234_XYZ",
            Guid.NewGuid(),
            "MRC20240101001",
            Guid.NewGuid(),
            "T1234567");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(TransactionStatus.Pending);
        result.Value.TransactionType.Should().Be(TransactionType.Sale);
    }

    [Fact]
    public void Create_ShouldGenerateReferenceNumber()
    {
        // Arrange & Act
        var transaction = CreateValidTransaction();

        // Assert
        transaction.ReferenceNumber.Should().NotBeNull();
        transaction.ReferenceNumber.Value.Should().HaveLength(12);
    }

    [Fact]
    public void Create_ShouldAddDomainEvent()
    {
        // Arrange & Act
        var transaction = CreateValidTransaction();

        // Assert
        transaction.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Create_RefundWithoutOriginalTransaction_ShouldFail()
    {
        // Arrange
        var amount = TransactionAmount.Create(500m, "TRY").Value!;

        // Act
        var result = TransactionAggregate.Create(
            TransactionType.Refund,
            amount,
            "4539 **** **** 1234",
            "ENC_4539_1234_XYZ",
            Guid.NewGuid(),
            "MRC20240101001",
            Guid.NewGuid(),
            "T1234567",
            null); // No original transaction

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("orijinal işlem");
    }

    [Fact]
    public void Create_RefundWithOriginalTransaction_ShouldSucceed()
    {
        // Arrange
        var amount = TransactionAmount.Create(500m, "TRY").Value!;
        var originalTransactionId = Guid.NewGuid();

        // Act
        var result = TransactionAggregate.Create(
            TransactionType.Refund,
            amount,
            "4539 **** **** 1234",
            "ENC_4539_1234_XYZ",
            Guid.NewGuid(),
            "MRC20240101001",
            Guid.NewGuid(),
            "T1234567",
            originalTransactionId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.OriginalTransactionId.Should().Be(originalTransactionId);
    }

    [Fact]
    public void Approve_FromPending_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.Approve();

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Approved);
        transaction.AuthorizationCode.Should().NotBeNull();
    }

    [Fact]
    public void Approve_ShouldGenerateAuthorizationCode()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.Approve();

        // Assert
        transaction.AuthorizationCode!.Value.Should().HaveLength(6);
    }

    [Fact]
    public void Approve_FromApproved_ShouldFail()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.Approve();

        // Act
        var result = transaction.Approve();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Decline_FromPending_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.Decline(DeclineReason.InsufficientLimit, "Yetersiz limit");

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Declined);
        transaction.DeclineReason.Should().Be(DeclineReason.InsufficientLimit);
        transaction.ErrorMessage.Should().Be("Yetersiz limit");
    }

    [Fact]
    public void Decline_FromApproved_ShouldFail()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.Approve();

        // Act
        var result = transaction.Decline(DeclineReason.InsufficientLimit);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Reverse_FromApproved_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.Approve();

        // Act
        var result = transaction.Reverse("Müşteri talebi");

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Reversed);
    }

    [Fact]
    public void Reverse_FromPending_ShouldFail()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.Reverse("Test");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Settle_FromApproved_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.Approve();

        // Act
        var result = transaction.Settle("BATCH-2024-001");

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Settled);
        transaction.BatchNumber.Should().Be("BATCH-2024-001");
        transaction.SettledAt.Should().NotBeNull();
    }

    [Fact]
    public void Settle_FromPending_ShouldFail()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.Settle("BATCH-2024-001");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetFraudCheckResult_Pass_ShouldNotDecline()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.SetFraudCheckResult(FraudCheckResult.Pass, 20);

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Pending);
        transaction.FraudCheckResult.Should().Be(FraudCheckResult.Pass);
        transaction.FraudScore.Should().Be(20);
    }

    [Fact]
    public void SetFraudCheckResult_Reject_ShouldDecline()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.SetFraudCheckResult(FraudCheckResult.Reject, 95);

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Declined);
        transaction.DeclineReason.Should().Be(DeclineReason.FraudSuspected);
    }

    [Fact]
    public void Approve_AfterFraudReject_ShouldFail()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.SetFraudCheckResult(FraudCheckResult.Reject, 95);

        // Act
        var result = transaction.Approve();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void MarkAsTimeout_FromPending_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.MarkAsTimeout();

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Timeout);
    }

    [Fact]
    public void MarkAsError_FromPending_ShouldSucceed()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        var result = transaction.MarkAsError("Sistem hatası");

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Status.Should().Be(TransactionStatus.Error);
        transaction.ErrorMessage.Should().Be("Sistem hatası");
    }
}