using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Events;

/// <summary>
/// Alert çözümlendiğinde fırlatılır.
/// Dinleyenler: CardFraudProfile güncelleme, Kart bloke tetikleme, Raporlama
/// </summary>
public class FraudAlertResolvedEvent : DomainEvent
{
    public Guid FraudAlertId { get; }
    public Guid TransactionId { get; }
    public string MaskedCardNo { get; }
    public FraudDecision Decision { get; }
    public string? CardStatusAction { get; }
    public string ResolvedBy { get; }
    public DateTime ResolvedAt { get; }

    public FraudAlertResolvedEvent(
        Guid fraudAlertId, Guid transactionId, string maskedCardNo,
        FraudDecision decision, string? cardStatusAction, string resolvedBy)
    {
        FraudAlertId = fraudAlertId;
        TransactionId = transactionId;
        MaskedCardNo = maskedCardNo;
        Decision = decision;
        CardStatusAction = cardStatusAction;
        ResolvedBy = resolvedBy;
        ResolvedAt = DateTime.UtcNow;
    }
}