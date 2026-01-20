using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Entities;

/// <summary>
/// Bloke doğrulama kaydı
/// </summary>
public class BlockVerification : Entity
{
    public Guid CardBlockId { get; private set; }

    // Doğrulama yöntemi
    public VerificationMethod Method { get; private set; } = null!;
    public VerificationResult VerificationResult { get; private set; } = null!;

    // OTP bilgileri
    public string? OtpCode { get; private set; }
    public DateTime? OtpSentAt { get; private set; }
    public DateTime? OtpExpiresAt { get; private set; }
    public int OtpAttempts { get; private set; }
    public const int MaxOtpAttempts = 3;

    // Tarihler
    public DateTime InitiatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Notlar
    public string? Notes { get; private set; }

    // Agent bilgisi (çağrı merkezi için)
    public string? AgentUsername { get; private set; }

    private BlockVerification() { }

    public static BlockVerification Create(Guid cardBlockId, VerificationMethod method)
    {
        var verification = new BlockVerification
        {
            CardBlockId = cardBlockId,
            Method = method,
            VerificationResult = Enums.VerificationResult.Pending,
            InitiatedAt = DateTime.UtcNow,
            OtpAttempts = 0
        };

        // OTP yöntemleri için kod oluştur
        if (method.IsRemote)
        {
            verification.GenerateOtp();
        }

        return verification;
    }

    /// <summary>
    /// OTP kodu oluştur
    /// </summary>
    public void GenerateOtp()
    {
        OtpCode = Random.Shared.Next(100000, 999999).ToString();
        OtpSentAt = DateTime.UtcNow;
        OtpExpiresAt = DateTime.UtcNow.AddMinutes(5);
        OtpAttempts = 0;
    }

    /// <summary>
    /// OTP doğrula
    /// </summary>
    public Result<bool> VerifyOtp(string enteredCode)
    {
        if (VerificationResult.IsFinal)
            return Result.Failure<bool>("Doğrulama zaten tamamlanmış");

        if (OtpExpiresAt.HasValue && DateTime.UtcNow > OtpExpiresAt.Value)
        {
            VerificationResult = Enums.VerificationResult.Expired;
            CompletedAt = DateTime.UtcNow;
            return Result.Failure<bool>("OTP süresi dolmuş");
        }

        OtpAttempts++;

        if (OtpCode == enteredCode)
        {
            VerificationResult = Enums.VerificationResult.Verified;
            CompletedAt = DateTime.UtcNow;
            return Result.Success(true);
        }

        if (OtpAttempts >= MaxOtpAttempts)
        {
            VerificationResult = Enums.VerificationResult.Failed;
            CompletedAt = DateTime.UtcNow;
            Notes = "Maksimum deneme sayısı aşıldı";
            return Result.Failure<bool>("Maksimum deneme sayısı aşıldı");
        }

        return Result.Failure<bool>($"Hatalı kod. Kalan deneme: {MaxOtpAttempts - OtpAttempts}");
    }

    /// <summary>
    /// Doğrulandı olarak işaretle (manuel)
    /// </summary>
    public void MarkAsVerified(string? notes, string? agentUsername = null)
    {
        VerificationResult = Enums.VerificationResult.Verified;
        CompletedAt = DateTime.UtcNow;
        Notes = notes;
        AgentUsername = agentUsername;
    }

    /// <summary>
    /// Başarısız olarak işaretle
    /// </summary>
    public void MarkAsFailed(string? notes, string? agentUsername = null)
    {
        VerificationResult = Enums.VerificationResult.Failed;
        CompletedAt = DateTime.UtcNow;
        Notes = notes;
        AgentUsername = agentUsername;
    }

    /// <summary>
    /// İptal et
    /// </summary>
    public void Cancel(string? reason)
    {
        VerificationResult = Enums.VerificationResult.Cancelled;
        CompletedAt = DateTime.UtcNow;
        Notes = reason;
    }

    /// <summary>
    /// OTP hala geçerli mi?
    /// </summary>
    public bool IsOtpValid()
    {
        return OtpExpiresAt.HasValue && DateTime.UtcNow <= OtpExpiresAt.Value && OtpAttempts < MaxOtpAttempts;
    }
}