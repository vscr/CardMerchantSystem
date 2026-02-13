using CardMerchantSystem.Shared.Kernel;

namespace Fraud.Domain.Events;

/// <summary>
/// Fraud sebebiyle kart bloke edildiğinde fırlatılır.
/// Dinleyenler: Card modülü (kart durumu güncelleme), Notification (müşteriye SMS)
/// </summary>
public class CardBlockedByFraudEvent : DomainEvent
{
    public Guid FraudAlertId { get; }
    public string MaskedCardNo { get; }
    public string ReasonCode { get; }
    public string BlockedBy { get; }
    public DateTime BlockedAt { get; }

    public CardBlockedByFraudEvent(
        Guid fraudAlertId, string maskedCardNo,
        string reasonCode, string blockedBy)
    {
        FraudAlertId = fraudAlertId;
        MaskedCardNo = maskedCardNo;
        ReasonCode = reasonCode;
        BlockedBy = blockedBy;
        BlockedAt = DateTime.UtcNow;
    }
}