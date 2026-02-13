using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Tetiklenen senaryo kaydı. PayGuard'daki HitScenario karşılığı.
/// Bir işlem birden fazla senaryoyu tetikleyebilir.
/// Her tetikleme ayrı bir HitScenario kaydı oluşturur.
/// </summary>
public class HitScenario : Entity
{
    public Guid TransactionId { get; private set; }
    public Guid FraudScenarioId { get; private set; }
    public string MaskedCardNo { get; private set; } = null!;
    public string? MerchantId { get; private set; }
    public int Score { get; private set; }
    public string FraudResponseCode { get; private set; } = null!;
    public bool IsOnline { get; private set; }
    public bool IsSimulation { get; private set; }
    public long ExecutionTimeMs { get; private set; }
    public DateTime DetectedAt { get; private set; }

    // Navigation
    public FraudScenario FraudScenario { get; private set; } = null!;

    private HitScenario() { } // EF Core

    public HitScenario(
        Guid transactionId, Guid fraudScenarioId,
        string maskedCardNo, string? merchantId,
        int score, string fraudResponseCode,
        bool isOnline, bool isSimulation, long executionTimeMs)
    {
        TransactionId = transactionId;
        FraudScenarioId = fraudScenarioId;
        MaskedCardNo = maskedCardNo;
        MerchantId = merchantId;
        Score = score;
        FraudResponseCode = fraudResponseCode;
        IsOnline = isOnline;
        IsSimulation = isSimulation;
        ExecutionTimeMs = executionTimeMs;
        DetectedAt = DateTime.UtcNow;
    }
}