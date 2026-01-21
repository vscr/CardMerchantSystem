using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Entities;

/// <summary>
/// Kart bloğu
/// </summary>
public class CardBlock : AggregateRoot
{
    public string BlockNumber { get; private set; } = null!;

    // Kart bilgileri
    public Guid CardId { get; private set; }
    public string CardNumberMasked { get; private set; } = null!;

    // Müşteri bilgileri
    public string CustomerName { get; private set; } = null!;
    public string CustomerPhone { get; private set; } = null!;
    public string CustomerEmail { get; private set; } = null!;

    // Bloke detayları
    public BlockReason Reason { get; private set; } = null!;
    public BlockStatus Status { get; private set; } = null!;
    public AlertSeverity Severity { get; private set; } = null!;

    // İlişkili kayıtlar
    public Guid? FraudAlertId { get; private set; }
    public Guid? TransactionId { get; private set; }

    // Tarihler
    public DateTime BlockedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // Çözüm bilgileri
    public string? ResolvedBy { get; private set; }
    public string? ResolutionNotes { get; private set; }

    // Bildirim durumu
    public bool SmsNotificationSent { get; private set; }
    public bool EmailNotificationSent { get; private set; }

    // Doğrulamalar
    private readonly List<BlockVerification> _verifications = new();
    public IReadOnlyCollection<BlockVerification> Verifications => _verifications.AsReadOnly();

    private CardBlock() { }

    public static Result<CardBlock> Create(
        Guid cardId,
        string cardNumberMasked,
        string customerName,
        string customerPhone,
        string customerEmail,
        BlockReason reason,
        AlertSeverity severity,
        int? blockDurationMinutes = null,
        Guid? fraudAlertId = null,
        Guid? transactionId = null)
    {
        if (cardId == Guid.Empty)
            return Result.Failure<CardBlock>("Kart ID boş olamaz");

        var block = new CardBlock
        {
            BlockNumber = GenerateBlockNumber(),
            CardId = cardId,
            CardNumberMasked = cardNumberMasked,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            Reason = reason,
            Status = BlockStatus.Active,
            Severity = severity,
            FraudAlertId = fraudAlertId,
            TransactionId = transactionId,
            BlockedAt = DateTime.UtcNow,
            ExpiresAt = blockDurationMinutes.HasValue ? DateTime.UtcNow.AddMinutes(blockDurationMinutes.Value) : null,
            SmsNotificationSent = false,
            EmailNotificationSent = false
        };

        return block;
    }

    /// <summary>
    /// Doğrulama başlat
    /// </summary>
    public Result<BlockVerification> InitiateVerification(VerificationMethod method, string operatorUsername)
    {
        if (!Status.CanResolve)
            return Result.Failure<BlockVerification>($"Bu durumda doğrulama başlatılamaz. Mevcut durum: {Status.DisplayName}");

        Status = BlockStatus.PendingVerification;

        var verification = BlockVerification.Create(Id, method);
        _verifications.Add(verification);

        MarkAsUpdated(operatorUsername);

        return verification;
    }

    /// <summary>
    /// Doğrulamayı tamamla
    /// </summary>
    public Result CompleteVerification(Guid verificationId, bool isSuccess, string? notes, string operatorUsername)
    {
        var verification = _verifications.FirstOrDefault(v => v.Id == verificationId);
        if (verification is null)
            return Result.Failure("Doğrulama bulunamadı");

        if (isSuccess)
        {
            verification.MarkAsVerified(notes);
        }
        else
        {
            verification.MarkAsFailed(notes);
        }

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Bloğu çöz
    /// </summary>
    public Result Resolve(string resolvedBy, string? resolutionNotes)
    {
        if (!Status.CanResolve)
            return Result.Failure($"Bu durumda çözülemez. Mevcut durum: {Status.DisplayName}");

        // En az bir başarılı doğrulama olmalı
        var hasSuccessfulVerification = _verifications.Any(v => v.VerificationResult == Enums.VerificationResult.Verified);
        if (!hasSuccessfulVerification)
            return Result.Failure("Bloğu çözmek için en az bir başarılı doğrulama gerekli");

        Status = BlockStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionNotes = resolutionNotes;

        return Result.Success();
    }

    /// <summary>
    /// Üst seviyeye ilet
    /// </summary>
    public Result Escalate(string reason, string operatorUsername)
    {
        if (!Status.CanEscalate)
            return Result.Failure($"Bu durumda üst seviyeye iletilemez. Mevcut durum: {Status.DisplayName}");

        Status = BlockStatus.Escalated;
        ResolutionNotes = string.IsNullOrEmpty(ResolutionNotes)
            ? $"Üst seviyeye iletildi: {reason}"
            : $"{ResolutionNotes}\nÜst seviyeye iletildi: {reason}";

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Kalıcı bloke yap
    /// </summary>
    public Result MakePermanent(string reason, string operatorUsername)
    {
        if (Status.IsFinal)
            return Result.Failure($"Bu durumda kalıcı yapılamaz. Mevcut durum: {Status.DisplayName}");

        Status = BlockStatus.PermanentBlock;
        ExpiresAt = null;
        ResolutionNotes = string.IsNullOrEmpty(ResolutionNotes)
            ? $"Kalıcı bloke: {reason}"
            : $"{ResolutionNotes}\nKalıcı bloke: {reason}";

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Süre dolduğunda
    /// </summary>
    public Result MarkAsExpired()
    {
        if (Status.IsFinal)
            return Result.Failure("Bloke zaten sonlanmış");

        if (ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value)
        {
            Status = BlockStatus.Expired;
            return Result.Success();
        }

        return Result.Failure("Bloke süresi henüz dolmadı");
    }

    /// <summary>
    /// SMS bildirimi gönderildi
    /// </summary>
    public void MarkSmsNotificationSent()
    {
        SmsNotificationSent = true;
    }

    /// <summary>
    /// Email bildirimi gönderildi
    /// </summary>
    public void MarkEmailNotificationSent()
    {
        EmailNotificationSent = true;
    }

    /// <summary>
    /// Bloke aktif mi?
    /// </summary>
    public bool IsBlockActive()
    {
        if (!Status.IsActive)
            return false;

        if (ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value)
            return false;

        return true;
    }

    private static string GenerateBlockNumber()
    {
        return $"BLK{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}