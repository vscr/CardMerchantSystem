using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Fraud alert havuzu. PayGuard'daki AlertTrackingPool karşılığı.
/// Fraud operatörlerinin inceleme kuyruğu.
/// Bir işlem birden fazla senaryoyu tetiklerse tek bir alert oluşur
/// (en yüksek skorlu senaryo baz alınır).
/// </summary>
public class FraudAlert : Entity
{
    public Guid TransactionId { get; private set; }
    public string MaskedCardNo { get; private set; } = null!;
    public string? MerchantId { get; private set; }
    public string? MerchantName { get; private set; }
    public decimal TransactionAmount { get; private set; }
    public string? CurrencyCode { get; private set; }
    public int TotalScore { get; private set; }
    public int HitScenarioCount { get; private set; }
    public string HighestFraudResponseCode { get; private set; } = null!;
    public FraudAlertStatus Status { get; private set; }

    /// <summary>Atanan operatör (null = henüz atanmadı)</summary>
    public string? AssignedTo { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolutionNote { get; private set; }
    public FraudDecision? Decision { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private FraudAlert() { } // EF Core

    public FraudAlert(
        Guid transactionId, string maskedCardNo,
        string? merchantId, string? merchantName,
        decimal transactionAmount, string? currencyCode,
        int totalScore, int hitScenarioCount, string highestFraudResponseCode)
    {
        TransactionId = transactionId;
        MaskedCardNo = maskedCardNo;
        MerchantId = merchantId;
        MerchantName = merchantName;
        TransactionAmount = transactionAmount;
        CurrencyCode = currencyCode;
        TotalScore = totalScore;
        HitScenarioCount = hitScenarioCount;
        HighestFraudResponseCode = highestFraudResponseCode;
        Status = FraudAlertStatus.New;
        CreatedAt = DateTime.UtcNow;
    }

    public void AssignTo(string operatorUsername)
    {
        AssignedTo = operatorUsername;
        AssignedAt = DateTime.UtcNow;
        Status = FraudAlertStatus.Assigned;
    }

    public void StartReview()
    {
        Status = FraudAlertStatus.InProgress;
    }

    public void Resolve(FraudDecision decision, string note)
    {
        Decision = decision;
        ResolutionNote = note;
        ResolvedAt = DateTime.UtcNow;
        Status = FraudAlertStatus.Resolved;
    }

    public void Escalate(string note)
    {
        ResolutionNote = note;
        Status = FraudAlertStatus.Escalated;
    }
}