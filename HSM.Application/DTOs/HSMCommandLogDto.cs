namespace HSM.Application.DTOs;

/// <summary>
/// HSM Command Log DTO
/// </summary>
public class HSMCommandLogDto
{
    public Guid Id { get; set; }
    public Guid HSMDeviceId { get; set; }
    public string? HSMDeviceName { get; set; }
    public string CommandType { get; set; } = null!;
    public string CommandTypeDisplayName { get; set; } = null!;
    public string? ResponseCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int ExecutionTimeMs { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string? ReferenceId { get; set; }
    public string? CardNumberMasked { get; set; }
}