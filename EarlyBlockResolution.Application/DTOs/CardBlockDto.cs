namespace EarlyBlockResolution.Application.DTOs;

public class CardBlockDto
{
    public Guid Id { get; set; }
    public string BlockNumber { get; set; } = null!;
    public Guid CardId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string ReasonDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public string SeverityDisplayName { get; set; } = null!;
    public Guid? FraudAlertId { get; set; }
    public Guid? TransactionId { get; set; }
    public DateTime BlockedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
    public bool SmsNotificationSent { get; set; }
    public bool EmailNotificationSent { get; set; }
    public bool IsBlockActive { get; set; }
    public int VerificationCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CardBlockWithVerificationsDto : CardBlockDto
{
    public List<BlockVerificationDto> Verifications { get; set; } = new();
}

public class CreateCardBlockDto
{
    public Guid CardId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public int ReasonId { get; set; }
    public int SeverityId { get; set; }
    public int? BlockDurationMinutes { get; set; }
    public Guid? FraudAlertId { get; set; }
    public Guid? TransactionId { get; set; }
}