using HSM.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Entities;

/// <summary>
/// HSM Cihaz Tanımı
/// </summary>
public class HSMDevice : AggregateRoot
{
    public string DeviceName { get; private set; } = null!;
    public HSMDeviceType DeviceType { get; private set; } = null!;
    public string IpAddress { get; private set; } = null!;
    public int Port { get; private set; }
    public string? SecondaryIpAddress { get; private set; }
    public int? SecondaryPort { get; private set; }
    public HSMConnectionStatus Status { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public bool IsPrimary { get; private set; }
    public int TimeoutMs { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastHealthCheck { get; private set; }
    public DateTime? LastSuccessfulCommand { get; private set; }
    public string? HeaderLength { get; private set; }

    // EF Core için
    private HSMDevice() { }

    public static Result<HSMDevice> Create(
        string deviceName,
        HSMDeviceType deviceType,
        string ipAddress,
        int port,
        bool isPrimary = false,
        int timeoutMs = 30000,
        int retryCount = 3)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return Result.Failure<HSMDevice>("Cihaz adı boş olamaz");

        if (string.IsNullOrWhiteSpace(ipAddress))
            return Result.Failure<HSMDevice>("IP adresi boş olamaz");

        if (port <= 0 || port > 65535)
            return Result.Failure<HSMDevice>("Geçersiz port numarası");

        var device = new HSMDevice
        {
            DeviceName = deviceName,
            DeviceType = deviceType,
            IpAddress = ipAddress,
            Port = port,
            Status = HSMConnectionStatus.Disconnected,
            IsActive = true,
            IsPrimary = isPrimary,
            TimeoutMs = timeoutMs,
            RetryCount = retryCount,
            HeaderLength = "4"
        };

        return device;
    }

    public void SetSecondaryConnection(string ipAddress, int port)
    {
        SecondaryIpAddress = ipAddress;
        SecondaryPort = port;
    }

    public void UpdateStatus(HSMConnectionStatus status)
    {
        Status = status;
        if (status == HSMConnectionStatus.Connected)
            LastHealthCheck = DateTime.UtcNow;
    }

    public void RecordSuccessfulCommand()
    {
        LastSuccessfulCommand = DateTime.UtcNow;
        Status = HSMConnectionStatus.Connected;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetAsPrimary() => IsPrimary = true;
    public void SetAsSecondary() => IsPrimary = false;
}