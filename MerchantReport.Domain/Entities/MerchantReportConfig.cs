using MerchantReport.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Entities;

/// <summary>
/// Üye İşyeri Rapor Ayarları
/// </summary>
public class MerchantReportConfig : AggregateRoot
{
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;
    public ReportType ReportType { get; private set; } = null!;
    public ReportFormat ReportFormat { get; private set; } = null!;
    public DeliveryMethod DeliveryMethod { get; private set; } = null!;
    public ScheduleFrequency Frequency { get; private set; } = null!;

    // Zamanlama
    public int DayOfWeek { get; private set; } // 1-7 (Pazartesi-Pazar)
    public int DayOfMonth { get; private set; } // 1-28
    public TimeSpan RunTime { get; private set; } // Çalışma saati
    public DateTime? NextRunTime { get; private set; }
    public DateTime? LastRunTime { get; private set; }

    // Dağıtım Ayarları
    public string? EmailRecipients { get; private set; } // Virgülle ayrılmış
    public string? FtpHost { get; private set; }
    public int? FtpPort { get; private set; }
    public string? FtpUsername { get; private set; }
    public string? FtpPassword { get; private set; }
    public string? FtpPath { get; private set; }
    public bool UseSftp { get; private set; }

    // API Callback
    public string? CallbackUrl { get; private set; }
    public string? CallbackApiKey { get; private set; }

    public bool IsActive { get; private set; }

    // EF Core için
    private MerchantReportConfig() { }

    /// <summary>
    /// Yeni rapor ayarı oluşturur
    /// </summary>
    public static Result<MerchantReportConfig> Create(
        string merchantId,
        string merchantName,
        ReportType reportType,
        ReportFormat reportFormat,
        DeliveryMethod deliveryMethod,
        ScheduleFrequency frequency,
        TimeSpan runTime,
        int dayOfWeek = 1,
        int dayOfMonth = 1)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantReportConfig>("Üye işyeri ID boş olamaz");

        if (string.IsNullOrWhiteSpace(merchantName))
            return Result.Failure<MerchantReportConfig>("Üye işyeri adı boş olamaz");

        var config = new MerchantReportConfig
        {
            MerchantId = merchantId,
            MerchantName = merchantName,
            ReportType = reportType,
            ReportFormat = reportFormat,
            DeliveryMethod = deliveryMethod,
            Frequency = frequency,
            DayOfWeek = dayOfWeek,
            DayOfMonth = dayOfMonth,
            RunTime = runTime,
            IsActive = true
        };

        config.CalculateNextRunTime();

        return config;
    }

    /// <summary>
    /// Email ayarlarını günceller
    /// </summary>
    public void SetEmailDelivery(string recipients)
    {
        DeliveryMethod = DeliveryMethod.Email;
        EmailRecipients = recipients;
    }

    /// <summary>
    /// FTP ayarlarını günceller
    /// </summary>
    public void SetFtpDelivery(string host, int port, string username, string password, string path, bool useSftp = false)
    {
        DeliveryMethod = useSftp ? DeliveryMethod.SFTP : DeliveryMethod.FTP;
        FtpHost = host;
        FtpPort = port;
        FtpUsername = username;
        FtpPassword = password;
        FtpPath = path;
        UseSftp = useSftp;
    }

    /// <summary>
    /// API Callback ayarlarını günceller
    /// </summary>
    public void SetApiCallback(string callbackUrl, string apiKey)
    {
        DeliveryMethod = DeliveryMethod.API;
        CallbackUrl = callbackUrl;
        CallbackApiKey = apiKey;
    }

    /// <summary>
    /// Bir sonraki çalışma zamanını hesaplar
    /// </summary>
    public void CalculateNextRunTime()
    {
        if (Frequency == ScheduleFrequency.OnDemand)
        {
            NextRunTime = null;
            return;
        }

        NextRunTime = Frequency.GetNextRunTime(DateTime.UtcNow, DayOfWeek, DayOfMonth, RunTime);
    }

    /// <summary>
    /// Çalıştırıldı olarak işaretle
    /// </summary>
    public void MarkAsRun()
    {
        LastRunTime = DateTime.UtcNow;
        CalculateNextRunTime();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}