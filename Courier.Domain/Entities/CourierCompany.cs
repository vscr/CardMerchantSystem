using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Enums;

namespace Courier.Domain.Entities;

/// <summary>
/// Kurye firması
/// </summary>
public class CourierCompany : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public CourierCompanyType CompanyType { get; private set; } = null!;

    // İletişim bilgileri
    public string ContactPerson { get; private set; } = null!;
    public string ContactPhone { get; private set; } = null!;
    public string ContactEmail { get; private set; } = null!;

    // Entegrasyon bilgileri
    public string? ApiEndpoint { get; private set; }
    public string? ApiKey { get; private set; }
    public string? FtpHost { get; private set; }
    public string? FtpUsername { get; private set; }
    public string? FtpPath { get; private set; }

    // Takip URL şablonu (örn: https://kurye.com/track/{trackingNumber})
    public string? TrackingUrlTemplate { get; private set; }

    // Durum
    public bool IsActive { get; private set; }

    // SLA bilgileri
    public int StandardDeliveryDays { get; private set; }
    public int ExpressDeliveryDays { get; private set; }

    // Fiyatlandırma
    public decimal BasePrice { get; private set; }
    public decimal PricePerKg { get; private set; }

    private CourierCompany() { }

    public static Result<CourierCompany> Create(
        string code,
        string name,
        CourierCompanyType companyType,
        string contactPerson,
        string contactPhone,
        string contactEmail,
        int standardDeliveryDays,
        decimal basePrice)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<CourierCompany>("Firma kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<CourierCompany>("Firma adı boş olamaz");

        if (standardDeliveryDays <= 0)
            return Result.Failure<CourierCompany>("Teslimat süresi sıfırdan büyük olmalı");

        var company = new CourierCompany
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            CompanyType = companyType,
            ContactPerson = contactPerson,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail,
            StandardDeliveryDays = standardDeliveryDays,
            ExpressDeliveryDays = Math.Max(1, standardDeliveryDays / 2),
            BasePrice = basePrice,
            PricePerKg = 0,
            IsActive = true
        };

        return company;
    }

    /// <summary>
    /// API entegrasyon bilgilerini ayarlar
    /// </summary>
    public void SetApiCredentials(string endpoint, string apiKey)
    {
        ApiEndpoint = endpoint;
        ApiKey = apiKey;
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
    /// Takip URL şablonunu ayarlar
    /// </summary>
    public void SetTrackingUrlTemplate(string template)
    {
        TrackingUrlTemplate = template;
    }

    /// <summary>
    /// Fiyatlandırmayı günceller
    /// </summary>
    public void UpdatePricing(decimal basePrice, decimal pricePerKg)
    {
        BasePrice = basePrice;
        PricePerKg = pricePerKg;
    }

    /// <summary>
    /// SLA güncellemesi
    /// </summary>
    public void UpdateSLA(int standardDays, int expressDays)
    {
        StandardDeliveryDays = standardDays;
        ExpressDeliveryDays = expressDays;
    }

    /// <summary>
    /// Firmayı aktif eder
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Firmayı deaktif eder
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Takip URL'i oluşturur
    /// </summary>
    public string? GetTrackingUrl(string trackingNumber)
    {
        if (string.IsNullOrEmpty(TrackingUrlTemplate))
            return null;

        return TrackingUrlTemplate.Replace("{trackingNumber}", trackingNumber);
    }
}