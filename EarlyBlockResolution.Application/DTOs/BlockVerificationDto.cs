namespace EarlyBlockResolution.Application.DTOs;

public class BlockVerificationDto
{
    public Guid Id { get; set; }
    public Guid CardBlockId { get; set; }
    public string Method { get; set; } = null!;
    public string MethodDisplayName { get; set; } = null!;
    public string Result { get; set; } = null!;
    public string ResultDisplayName { get; set; } = null!;
    public DateTime? OtpSentAt { get; set; }
    public DateTime? OtpExpiresAt { get; set; }
    public int OtpAttempts { get; set; }
    public bool IsOtpValid { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public string? AgentUsername { get; set; }
}

public class InitiateVerificationDto
{
    public int MethodId { get; set; }
}

public class VerifyOtpDto
{
    public Guid VerificationId { get; set; }
    public string OtpCode { get; set; } = null!;
}

public class CompleteVerificationDto
{
    public Guid VerificationId { get; set; }
    public bool IsSuccess { get; set; }
    public string? Notes { get; set; }
}