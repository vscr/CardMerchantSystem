using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Entities;

/// <summary>
/// Fraud uyarısı
/// </summary>
public class FraudAlert : AggregateRoot
{
    public string AlertNumber { get; private set; } = null!;

    // Kart bilgileri
    public Guid CardId { get; private set; }
    public string CardNumberMasked { get; private set; } = null!;

    // İşlem bilgileri
    public Guid? TransactionId { get; private set; }
    public decimal? TransactionAmount { get; private set; }
    public string? MerchantName { get; private set; }

    // Uyarı detayları
    public BlockReason Reason { get; private set; } = null!;
    public AlertSeverity Severity { get; private set; } = null!;
    public int FraudScore { get; private set; }

    // Tetikleyen kural
    public Guid? BlockRuleId { get; private set; }
    public BlockRule? TriggerRule { get; private set; }

    // Durum
    public bool IsProcessed { get; private set; }
    public bool BlockCreated { get; private set; }
    public Guid? CardBlockId { get; private set; }

    // Tarihler
    public DateTime DetectedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // Notlar
    public string? Notes { get; private set; }

    private FraudAlert() { }

    public static Result<FraudAlert> Create(
        Guid cardId,
        string cardNumberMasked,
        BlockReason reason,
        AlertSeverity severity,
        int fraudScore,
        Guid? transactionId = null,
        decimal? transactionAmount = null,
        string? merchantName = null,
        Guid? blockRuleId = null)
    {
        if (cardId == Guid.Empty)
            return Result.Failure<FraudAlert>("Kart ID boş olamaz");

        var alert = new FraudAlert
        {
            AlertNumber = GenerateAlertNumber(),
            CardId = cardId,
            CardNumberMasked = cardNumberMasked,
            TransactionId = transactionId,
            TransactionAmount = transactionAmount,
            MerchantName = merchantName,
            Reason = reason,
            Severity = severity,
            FraudScore = fraudScore,
            BlockRuleId = blockRuleId,
            IsProcessed = false,
            BlockCreated = false,
            DetectedAt = DateTime.UtcNow
        };

        return alert;
    }

    /// <summary>
    /// Uyarıyı işlenmiş olarak işaretle
    /// </summary>
    public void MarkAsProcessed(bool blockCreated, Guid? cardBlockId, string? notes)
    {
        IsProcessed = true;
        ProcessedAt = DateTime.UtcNow;
        BlockCreated = blockCreated;
        CardBlockId = cardBlockId;
        Notes = notes;
    }

    /// <summary>
    /// Not ekle
    /// </summary>
    public void AddNote(string note)
    {
        Notes = string.IsNullOrEmpty(Notes) ? note : $"{Notes}\n{note}";
    }

    private static string GenerateAlertNumber()
    {
        return $"ALR{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}