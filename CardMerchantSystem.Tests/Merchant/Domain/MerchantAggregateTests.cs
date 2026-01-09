using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;
using FluentAssertions;

namespace CardMerchantSystem.Tests.Merchant.Domain;

public class MerchantAggregateTests
{
    private MerchantAggregate CreateValidMerchant()
    {
        var taxNumber = TaxNumber.Create("1234567890").Value!;
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        var result = MerchantAggregate.Create(
            "ABC Market",
            "ABC Gıda Ticaret Ltd. Şti.",
            taxNumber,
            "Konak",
            MerchantType.LimitedCompany,
            "5551234567",
            "info@abcmarket.com",
            "Atatürk Caddesi No:50",
            "İzmir",
            "Konak",
            iban,
            1.75m);

        return result.Value!;
    }

    [Fact]
    public void Create_ValidData_ShouldReturnSuccess()
    {
        // Arrange
        var taxNumber = TaxNumber.Create("1234567890").Value!;
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        // Act
        var result = MerchantAggregate.Create(
            "ABC Market",
            "ABC Gıda Ticaret Ltd. Şti.",
            taxNumber,
            "Konak",
            MerchantType.LimitedCompany,
            "5551234567",
            "info@abcmarket.com",
            "Atatürk Caddesi No:50",
            "İzmir",
            "Konak",
            iban,
            1.75m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(MerchantStatus.Pending);
        result.Value.Name.Should().Be("ABC Market");
    }

    [Fact]
    public void Create_ShouldGenerateMerchantCode()
    {
        // Arrange & Act
        var merchant = CreateValidMerchant();

        // Assert
        merchant.MerchantCode.Should().NotBeNull();
        merchant.MerchantCode.Value.Should().StartWith("MRC");
    }

    [Fact]
    public void Create_ShouldAddDomainEvent()
    {
        // Arrange & Act
        var merchant = CreateValidMerchant();

        // Assert
        merchant.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Create_InvalidCommissionRate_ShouldFail()
    {
        // Arrange
        var taxNumber = TaxNumber.Create("1234567890").Value!;
        var iban = IBAN.Create("TR330006100519786457841326").Value!;

        // Act
        var result = MerchantAggregate.Create(
            "ABC Market",
            "ABC Gıda Ticaret Ltd. Şti.",
            taxNumber,
            "Konak",
            MerchantType.LimitedCompany,
            "5551234567",
            "info@abcmarket.com",
            "Atatürk Caddesi No:50",
            "İzmir",
            "Konak",
            iban,
            150m); // Invalid: > 100

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void StartReview_FromPending_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();

        // Act
        var result = merchant.StartReview("admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.UnderReview);
    }

    [Fact]
    public void Approve_FromUnderReview_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.StartReview("admin");

        // Act
        var result = merchant.Approve("admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Approved);
        merchant.ApprovedBy.Should().Be("admin");
        merchant.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_FromPending_ShouldAutoStartReview()
    {
        // Arrange
        var merchant = CreateValidMerchant();

        // Act
        var result = merchant.Approve("admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Approved);
    }

    [Fact]
    public void Reject_FromUnderReview_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.StartReview("admin");

        // Act
        var result = merchant.Reject("Eksik belgeler", "admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Rejected);
        merchant.RejectionReason.Should().Be("Eksik belgeler");
    }

    [Fact]
    public void Reject_WithoutReason_ShouldFail()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.StartReview("admin");

        // Act
        var result = merchant.Reject("", "admin");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Activate_FromApproved_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.Approve("admin");

        // Act
        var result = merchant.Activate("operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Active);
        merchant.ContractStartDate.Should().NotBeNull();
    }

    [Fact]
    public void Suspend_FromActive_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.Approve("admin");
        merchant.Activate("operator");

        // Act
        var result = merchant.Suspend("Şüpheli işlem", "operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Suspended);
    }

    [Fact]
    public void Close_FromActive_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.Approve("admin");
        merchant.Activate("operator");

        // Act
        var result = merchant.Close("operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.Status.Should().Be(MerchantStatus.Closed);
        merchant.ContractEndDate.Should().NotBeNull();
    }

    [Fact]
    public void AddTerminal_ToActiveMerchant_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.Approve("admin");
        merchant.Activate("operator");

        // Act
        var result = merchant.AddTerminal(TerminalType.Ingenico, "ING-001", "Move 5000", "Kasa 1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TerminalType.Should().Be(TerminalType.Ingenico);
        merchant.Terminals.Should().HaveCount(1);
    }

    [Fact]
    public void AddTerminal_ToPendingMerchant_ShouldFail()
    {
        // Arrange
        var merchant = CreateValidMerchant();

        // Act
        var result = merchant.AddTerminal(TerminalType.Ingenico, "ING-001", "Move 5000", "Kasa 1");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ActivateTerminal_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();
        merchant.Approve("admin");
        merchant.Activate("operator");
        var terminalResult = merchant.AddTerminal(TerminalType.Ingenico, "ING-001", "Move 5000", "Kasa 1");
        var terminalId = terminalResult.Value!.Id;

        // Act
        var result = merchant.ActivateTerminal(terminalId, "operator");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.ActiveTerminalCount.Should().Be(1);
    }

    [Fact]
    public void UpdateCommissionRate_ValidRate_ShouldSucceed()
    {
        // Arrange
        var merchant = CreateValidMerchant();

        // Act
        var result = merchant.UpdateCommissionRate(2.5m, "admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.CommissionRate.Should().Be(2.5m);
    }

    [Fact]
    public void UpdateCommissionRate_InvalidRate_ShouldFail()
    {
        // Arrange
        var merchant = CreateValidMerchant();

        // Act
        var result = merchant.UpdateCommissionRate(150m, "admin");

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}