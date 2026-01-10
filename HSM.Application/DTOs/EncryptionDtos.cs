namespace HSM.Application.DTOs;

/// <summary>
/// Data şifreleme request
/// </summary>
public class EncryptDataRequestDto
{
    public string PlainData { get; set; } = null!;
}

/// <summary>
/// Data şifreleme response
/// </summary>
public class EncryptDataResponseDto
{
    public bool IsSuccess { get; set; }
    public string? EncryptedData { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Data çözme request
/// </summary>
public class DecryptDataRequestDto
{
    public string EncryptedData { get; set; } = null!;
}

/// <summary>
/// Data çözme response
/// </summary>
public class DecryptDataResponseDto
{
    public bool IsSuccess { get; set; }
    public string? PlainData { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Health check response
/// </summary>
public class HSMHealthCheckResponseDto
{
    public bool IsHealthy { get; set; }
    public string DeviceName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int ResponseTimeMs { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? ErrorMessage { get; set; }
}