namespace Statement.Application.DTOs;

/// <summary>
/// Bildirim DTO
/// </summary>
public class StatementNotificationDto
{
    public Guid Id { get; set; }
    public Guid StatementId { get; set; }
    public string NotificationType { get; set; } = null!;
    public string NotificationTypeDisplayName { get; set; } = null!;
    public string Recipient { get; set; } = null!;
    public string? Subject { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// Bildirim Gönderme DTO
/// </summary>
public class SendStatementNotificationDto
{
    public Guid StatementId { get; set; }
    public int NotificationTypeId { get; set; }
    public string? Recipient { get; set; }
}