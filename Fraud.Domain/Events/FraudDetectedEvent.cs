using CardMerchantSystem.Shared.Kernel;

namespace Fraud.Domain.Events;

/// <summary>
/// Fraud tespit edildiğinde fırlatılır.
/// Dinleyenler: FraudAlert oluşturma, Notification gönderme, CardFraudProfile güncelleme
/// </summary>
public class FraudDetectedEvent : DomainEvent
{
    public Guid TransactionId { get; }
    public Guid FraudScenarioId { get; }
    public string MaskedCardNo { get; }
    public string? MerchantId { get; }
    public string? MerchantName { get; }
    public decimal TransactionAmount { get; }
    public string? CurrencyCode { get; }
    public int Score { get; }
    public string FraudResponseCode { get; }
    public bool IsSimulation { get; }
    public bool IsOnline { get; }
    public long ExecutionTimeMs { get; }
    public DateTime DetectedAt { get; }

    public FraudDetectedEvent(
        Guid transactionId, Guid fraudScenarioId,
        string maskedCardNo, string? merchantId, string? merchantName,
        decimal transactionAmount, string? currencyCode,
        int score, string fraudResponseCode,
        bool isSimulation, bool isOnline, long executionTimeMs)
    {
        TransactionId = transactionId;
        FraudScenarioId = fraudScenarioId;
        MaskedCardNo = maskedCardNo;
        MerchantId = merchantId;
        MerchantName = merchantName;
        TransactionAmount = transactionAmount;
        CurrencyCode = currencyCode;
        Score = score;
        FraudResponseCode = fraudResponseCode;
        IsSimulation = isSimulation;
        IsOnline = isOnline;
        ExecutionTimeMs = executionTimeMs;
        DetectedAt = DateTime.UtcNow;
    }
}