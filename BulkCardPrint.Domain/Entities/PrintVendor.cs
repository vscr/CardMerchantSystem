using CardMerchantSystem.Shared.Kernel;
using BulkCardPrint.Domain.Enums;

namespace BulkCardPrint.Domain.Entities;

/// <summary>
/// Kart basım firması
/// </summary>
public class PrintVendor : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string ContactPerson { get; private set; } = null!;
    public string ContactEmail { get; private set; } = null!;
    public string ContactPhone { get; private set; } = null!;

    // Entegrasyon bilgileri
    public string? ApiEndpoint { get; private set; }
    public string? FtpHost { get; private set; }
    public string? FtpUsername { get; private set; }
    public string? FtpPath { get; private set; }
    public FileFormat PreferredFileFormat { get; private set; } = null!;

    // Durum
    public bool IsActive { get; private set; }

    // Kapasite
    public int DailyCapacity { get; private set; }
    public int CurrentDailyLoad { get; private set; }

    private PrintVendor() { }

    public static Result<PrintVendor> Create(
        string code,
        string name,
        string contactPerson,
        string contactEmail,
        string contactPhone,
        FileFormat preferredFileFormat,
        int dailyCapacity)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<PrintVendor>("Firma kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<PrintVendor>("Firma adı boş olamaz");

        if (dailyCapacity <= 0)
            return Result.Failure<PrintVendor>("Günlük kapasite sıfırdan büyük olmalı");

        var vendor = new PrintVendor
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            ContactPerson = contactPerson,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            PreferredFileFormat = preferredFileFormat,
            DailyCapacity = dailyCapacity,
            CurrentDailyLoad = 0,
            IsActive = true
        };

        return vendor;
    }

    /// <summary>
    /// FTP bilgilerini ayarlar
    /// </summary>
    public void SetFtpCredentials(string host, string username, string path)
    {
        FtpHost = host;
        FtpUsername = username;
        FtpPath = path;
    }

    /// <summary>
    /// API endpoint ayarlar
    /// </summary>
    public void SetApiEndpoint(string endpoint)
    {
        ApiEndpoint = endpoint;
    }

    /// <summary>
    /// Günlük yükü artırır
    /// </summary>
    public Result AddToLoad(int count)
    {
        if (CurrentDailyLoad + count > DailyCapacity)
            return Result.Failure($"Günlük kapasite aşılıyor. Mevcut: {CurrentDailyLoad}, Kapasite: {DailyCapacity}");

        CurrentDailyLoad += count;
        return Result.Success();
    }

    /// <summary>
    /// Günlük yükü sıfırlar
    /// </summary>
    public void ResetDailyLoad()
    {
        CurrentDailyLoad = 0;
    }

    /// <summary>
    /// Firmayı deaktif eder
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Firmayı aktif eder
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Kalan kapasite
    /// </summary>
    public int RemainingCapacity => DailyCapacity - CurrentDailyLoad;
}