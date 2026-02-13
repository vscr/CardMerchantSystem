using Fraud.Domain.Enums;

namespace Fraud.Application.Models;

/// <summary>
/// Fraud kontrol sonucu. Engine'den döner.
/// Transaction modülüne geri iletilir.
/// </summary>
public class FraudCheckResult
{
    public Guid TransactionId { get; set; }
    public FraudStatus Status { get; set; }
    public string FraudResponseCode { get; set; } = "00";
    public int TotalScore { get; set; }
    public int HitScenarioCount { get; set; }
    public List<HitScenarioResult> HitScenarios { get; set; } = new();
    public long TotalExecutionTimeMs { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    public bool IsFraudulent => Status == FraudStatus.Fraudulent;
    public bool IsSuspicious => Status == FraudStatus.Suspicious;
    public bool IsClean => Status == FraudStatus.Clean;

    public static FraudCheckResult Clean(Guid transactionId, long executionMs) => new()
    {
        TransactionId = transactionId,
        Status = FraudStatus.Clean,
        FraudResponseCode = "00",
        TotalScore = 0,
        TotalExecutionTimeMs = executionMs
    };
}

public class HitScenarioResult
{
    public Guid ScenarioId { get; set; }
    public string ScenarioName { get; set; } = null!;
    public int Score { get; set; }
    public string FraudResponseCode { get; set; } = null!;
    public bool IsSimulation { get; set; }
    public long ExecutionTimeMs { get; set; }
}