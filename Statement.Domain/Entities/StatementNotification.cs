using Statement.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Entities;

/// <summary>
/// Ekstre Bildirimi
/// </summary>
public class StatementNotification : Entity
{
    public Guid StatementId { get; private set; }
    public NotificationType NotificationType { get; private set; } = null!;
    public string Recipient { get; private set; } = null!;
    public string? Subject { get; private set; }
    public string? Content { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    // EF Core için
    private StatementNotification() { }

    /// <summary>
    /// Yeni bildirim oluşturur
    /// </summary>
    public static StatementNotification Create(
        Guid statementId,
        NotificationType notificationType,
        string recipient,
        string? subject = null,
        string? content = null)
    {
        return new StatementNotification
        {
            StatementId = statementId,
            NotificationType = notificationType,
            Recipient = recipient,
            Subject = subject,
            Content = content,
            IsSent = false,
            RetryCount = 0
        };
    }

    /// <summary>
    /// Gönderildi olarak işaretle
    /// </summary>
    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    /// <summary>
    /// Hata kaydet
    /// </summary>
    public void RecordError(string errorMessage)
    {
        ErrorMessage = errorMessage;
        RetryCount++;
    }
}