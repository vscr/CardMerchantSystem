namespace MerchantReport.Application.DTOs;

/// <summary>
/// Üye İşyeri Rapor Ayarı DTO
/// </summary>
public class MerchantReportConfigDto
{
    public Guid Id { get; set; }
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public string ReportType { get; set; } = null!;
    public string ReportTypeDisplayName { get; set; } = null!;
    public string ReportFormat { get; set; } = null!;
    public string DeliveryMethod { get; set; } = null!;
    public string DeliveryMethodDisplayName { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string FrequencyDisplayName { get; set; } = null!;
    public int DayOfWeek { get; set; }
    public int DayOfMonth { get; set; }
    public string RunTime { get; set; } = null!;
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }
    public string? EmailRecipients { get; set; }
    public string? FtpHost { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Rapor Ayarı Oluşturma DTO
/// </summary>
public class CreateMerchantReportConfigDto
{
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public int ReportTypeId { get; set; }
    public int ReportFormatId { get; set; }
    public int DeliveryMethodId { get; set; }
    public int FrequencyId { get; set; }
    public int DayOfWeek { get; set; } = 1;
    public int DayOfMonth { get; set; } = 1;
    public string RunTime { get; set; } = "06:00";
}

/// <summary>
/// Email Dağıtım Ayarı DTO
/// </summary>
public class SetEmailDeliveryDto
{
    public Guid ConfigId { get; set; }
    public string Recipients { get; set; } = null!;
}

/// <summary>
/// FTP Dağıtım Ayarı DTO
/// </summary>
public class SetFtpDeliveryDto
{
    public Guid ConfigId { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 21;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Path { get; set; } = "/";
    public bool UseSftp { get; set; } = false;
}