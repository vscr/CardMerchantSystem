namespace HSM.Application.DTOs;

/// <summary>
/// HSM Device DTO
/// </summary>
public class HSMDeviceDto
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = null!;
    public string DeviceType { get; set; } = null!;
    public string DeviceTypeDisplayName { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public string? SecondaryIpAddress { get; set; }
    public int? SecondaryPort { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public int TimeoutMs { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public DateTime? LastSuccessfulCommand { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// HSM Device oluşturma DTO
/// </summary>
public class CreateHSMDeviceDto
{
    public string DeviceName { get; set; } = null!;
    public int DeviceTypeId { get; set; }
    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public string? SecondaryIpAddress { get; set; }
    public int? SecondaryPort { get; set; }
    public bool IsPrimary { get; set; }
    public int TimeoutMs { get; set; } = 30000;
    public int RetryCount { get; set; } = 3;
}