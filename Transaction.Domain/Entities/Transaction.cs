using Transaction.Domain.Enums;
using Transaction.Domain.Events;
using Transaction.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Entities;

/// <summary>
/// İşlem Aggregate Root
/// </summary>
public class TransactionAggregate : AggregateRoot
{
    // İşlem Bilgileri
    public ReferenceNumber ReferenceNumber { get; private set; } = null!;
    public TransactionType TransactionType { get; private set; } = null!;
    public TransactionStatus Status { get; private set; } = null!;
    public TransactionAmount Amount { get; private set; } = null!;
    public AuthorizationCode? AuthorizationCode { get; private set; }

    // Kart Bilgileri
    public string CardNumberMasked { get; private set; } = null!;
    public string CardNumberEncrypted { get; private set; } = null!;

    // Üye İşyeri & Terminal
    public Guid MerchantId { get; private set; }
    public string MerchantCode { get; private set; } = null!;
    public Guid TerminalId { get; private set; }
    public string TerminalCode { get; private set; } = null!;

    // Red/Hata Bilgileri
    public DeclineReason? DeclineReason { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Fraud Bilgileri
    public FraudCheckResult? FraudCheckResult { get; private set; }
    public int? FraudScore { get; private set; }

    // Orijinal İşlem (İade/İptal için)
    public Guid? OriginalTransactionId { get; private set; }

    // Takas Bilgileri
    public DateTime? SettledAt { get; private set; }
    public string? BatchNumber { get; private set; }

    // EF Core için
    private TransactionAggregate() { }

    /// <summary>
    /// Yeni işlem oluşturur
    /// </summary>
    public static Result<TransactionAggregate> Create(
        TransactionType transactionType,
        TransactionAmount amount,
        string cardNumberMasked,
        string cardNumberEncrypted,
        Guid merchantId,
        string merchantCode,
        Guid terminalId,
        string terminalCode,
        Guid? originalTransactionId = null)
    {
        if (string.IsNullOrWhiteSpace(cardNumberMasked))
            return Result.Failure<TransactionAggregate>("Kart numarası boş olamaz");

        if (string.IsNullOrWhiteSpace(merchantCode))
            return Result.Failure<TransactionAggregate>("Üye işyeri kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(terminalCode))
            return Result.Failure<TransactionAggregate>("Terminal kodu boş olamaz");

        // İade/İptal için orijinal işlem zorunlu
        if ((transactionType == TransactionType.Refund || transactionType == TransactionType.Cancel)
            && !originalTransactionId.HasValue)
            return Result.Failure<TransactionAggregate>("İade/İptal işlemleri için orijinal işlem belirtilmeli");

        var transaction = new TransactionAggregate
        {
            ReferenceNumber = ReferenceNumber.Generate(),
            TransactionType = transactionType,
            Status = TransactionStatus.Pending,
            Amount = amount,
            CardNumberMasked = cardNumberMasked,
            CardNumberEncrypted = cardNumberEncrypted,
            MerchantId = merchantId,
            MerchantCode = merchantCode,
            TerminalId = terminalId,
            TerminalCode = terminalCode,
            OriginalTransactionId = originalTransactionId
        };

        transaction.AddDomainEvent(new TransactionCreatedEvent(
            transaction.Id,
            transaction.ReferenceNumber.Value,
            amount.Amount,
            cardNumberMasked));

        return transaction;
    }

    /// <summary>
    /// Fraud kontrolü sonucunu ayarlar
    /// </summary>
    public Result SetFraudCheckResult(FraudCheckResult result, int score)
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("Sadece bekleyen işlemlerde fraud kontrolü yapılabilir");

        FraudCheckResult = result;
        FraudScore = score;

        if (result == Enums.FraudCheckResult.Reject)
        {
            Status = TransactionStatus.Declined;
            DeclineReason = Enums.DeclineReason.FraudSuspected;

            AddDomainEvent(new FraudDetectedEvent(
                Id,
                ReferenceNumber.Value,
                CardNumberMasked,
                Amount.Amount,
                "Fraud skoru çok yüksek"));

            AddDomainEvent(new TransactionDeclinedEvent(
                Id,
                ReferenceNumber.Value,
                Enums.DeclineReason.FraudSuspected.DisplayName));
        }

        return Result.Success();
    }

    /// <summary>
    /// İşlemi onaylar
    /// </summary>
    public Result Approve()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("Sadece bekleyen işlemler onaylanabilir");

        if (FraudCheckResult == Enums.FraudCheckResult.Reject)
            return Result.Failure("Fraud kontrolünden geçemeyen işlem onaylanamaz");

        Status = TransactionStatus.Approved;
        AuthorizationCode = ValueObjects.AuthorizationCode.Generate();
        MarkAsUpdated();

        AddDomainEvent(new TransactionApprovedEvent(
            Id,
            ReferenceNumber.Value,
            AuthorizationCode.Value,
            Amount.Amount));

        return Result.Success();
    }

    /// <summary>
    /// İşlemi reddeder
    /// </summary>
    public Result Decline(DeclineReason reason, string? errorMessage = null)
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("Sadece bekleyen işlemler reddedilebilir");

        Status = TransactionStatus.Declined;
        DeclineReason = reason;
        ErrorMessage = errorMessage;
        MarkAsUpdated();

        AddDomainEvent(new TransactionDeclinedEvent(
            Id,
            ReferenceNumber.Value,
            reason.DisplayName));

        return Result.Success();
    }

    /// <summary>
    /// İşlemi iptal eder (Reverse)
    /// </summary>
    public Result Reverse(string reason)
    {
        if (Status != TransactionStatus.Approved)
            return Result.Failure("Sadece onaylı işlemler iptal edilebilir");

        Status = TransactionStatus.Reversed;
        ErrorMessage = reason;
        MarkAsUpdated();

        return Result.Success();
    }

    /// <summary>
    /// İşlemi takas eder
    /// </summary>
    public Result Settle(string batchNumber)
    {
        if (Status != TransactionStatus.Approved)
            return Result.Failure("Sadece onaylı işlemler takas edilebilir");

        Status = TransactionStatus.Settled;
        SettledAt = DateTime.UtcNow;
        BatchNumber = batchNumber;
        MarkAsUpdated();

        AddDomainEvent(new TransactionSettledEvent(
            Id,
            ReferenceNumber.Value,
            SettledAt.Value));

        return Result.Success();
    }

    /// <summary>
    /// Hata olarak işaretle
    /// </summary>
    public Result MarkAsError(string errorMessage)
    {
        if (Status.IsFinal)
            return Result.Failure("Final durumdaki işlemler değiştirilemez");

        Status = TransactionStatus.Error;
        ErrorMessage = errorMessage;
        MarkAsUpdated();

        return Result.Success();
    }

    /// <summary>
    /// Timeout olarak işaretle
    /// </summary>
    public Result MarkAsTimeout()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("Sadece bekleyen işlemler timeout olabilir");

        Status = TransactionStatus.Timeout;
        ErrorMessage = "İşlem zaman aşımına uğradı";
        MarkAsUpdated();

        return Result.Success();
    }
}