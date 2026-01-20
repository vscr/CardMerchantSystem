using CardMerchantSystem.Shared.Kernel;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Domain.Entities;

/// <summary>
/// Rapor tanımı
/// </summary>
public class ReportDefinition : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Kurum ve tip
    public RegulatoryAuthority Authority { get; private set; } = null!;
    public ReportType ReportType { get; private set; } = null!;
    public ReportFileFormat FileFormat { get; private set; } = null!;

    // Periyot
    public ReportFrequency Frequency { get; private set; } = null!;

    // Şablon
    public string? TemplateQuery { get; private set; }
    public string? TemplateFilePath { get; private set; }

    // Durum
    public bool IsActive { get; private set; }

    // Son üretim
    public DateTime? LastGeneratedAt { get; private set; }
    public DateTime? NextScheduledAt { get; private set; }

    private ReportDefinition() { }

    public static Result<ReportDefinition> Create(
        string code,
        string name,
        string description,
        RegulatoryAuthority authority,
        ReportType reportType,
        ReportFileFormat fileFormat,
        ReportFrequency frequency)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<ReportDefinition>("Rapor kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ReportDefinition>("Rapor adı boş olamaz");

        var definition = new ReportDefinition
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            Authority = authority,
            ReportType = reportType,
            FileFormat = fileFormat,
            Frequency = frequency,
            IsActive = true
        };

        definition.CalculateNextSchedule();
        return definition;
    }

    /// <summary>
    /// Şablon sorgusu ayarlar
    /// </summary>
    public void SetTemplateQuery(string query)
    {
        TemplateQuery = query;
    }

    /// <summary>
    /// Şablon dosyası ayarlar
    /// </summary>
    public void SetTemplateFile(string filePath)
    {
        TemplateFilePath = filePath;
    }

    /// <summary>
    /// Rapor üretildi olarak işaretle
    /// </summary>
    public void MarkAsGenerated()
    {
        LastGeneratedAt = DateTime.UtcNow;
        CalculateNextSchedule();
    }

    /// <summary>
    /// Aktif et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        CalculateNextSchedule();
    }

    /// <summary>
    /// Deaktif et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        NextScheduledAt = null;
    }

    private void CalculateNextSchedule()
    {
        if (!IsActive || Frequency == ReportFrequency.OnDemand)
        {
            NextScheduledAt = null;
            return;
        }

        var baseDate = LastGeneratedAt ?? DateTime.UtcNow;
        NextScheduledAt = baseDate.AddDays(Frequency.GetDaysInterval());
    }
}