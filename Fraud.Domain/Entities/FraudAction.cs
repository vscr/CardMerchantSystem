using CardMerchantSystem.Shared.Kernel;
using Fraud.Domain.Enums;

namespace Fraud.Domain.Entities;

/// <summary>
/// Fraud operatör aksiyonu. PayGuard'daki FraudAction karşılığı.
/// Operatör alert'i inceleyip karar verdikten sonra alınan aksiyon.
/// Bir alert için birden fazla aksiyon alınabilir (örn: önce watchlist, sonra kart bloke).
/// </summary>
public class FraudAction : Entity
{
    public Guid FraudAlertId { get; private set; }
    public Guid TransactionId { get; private set; }
    public string MaskedCardNo { get; private set; } = null!;
    public FraudDecision Decision { get; private set; }

    /// <summary>İşlem yorumu (operatör notu)</summary>
    public string? Comment { get; private set; }

    /// <summary>Kart durumu değişikliği (varsa): Active, Blocked, Suspended</summary>
    public string? CardStatusAction { get; private set; }

    /// <summary>Kart durum değişiklik sebebi kodu</summary>
    public string? CardStatusReasonCode { get; private set; }

    /// <summary>Aksiyonu alan operatör</summary>
    public string ActionBy { get; private set; } = null!;
    public DateTime ActionAt { get; private set; }

    // Navigation
    public FraudAlert FraudAlert { get; private set; } = null!;

    private FraudAction() { } // EF Core

    public FraudAction(
        Guid fraudAlertId, Guid transactionId, string maskedCardNo,
        FraudDecision decision, string? comment,
        string? cardStatusAction, string? cardStatusReasonCode,
        string actionBy)
    {
        FraudAlertId = fraudAlertId;
        TransactionId = transactionId;
        MaskedCardNo = maskedCardNo;
        Decision = decision;
        Comment = comment;
        CardStatusAction = cardStatusAction;
        CardStatusReasonCode = cardStatusReasonCode;
        ActionBy = actionBy;
        ActionAt = DateTime.UtcNow;
    }
}