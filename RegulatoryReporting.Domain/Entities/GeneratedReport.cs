using CardMerchantSystem.Shared.Kernel;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Domain.Entities;

/// <summary>
/// Üretilen rapor
/// </summary>
public class GeneratedReport : AggregateRoot
{
    public string ReportNumber { get; private set; } = null!;
    public Guid ReportDefinitionId { get; private set; }
    public ReportDefinition Definition { get; private set; } = null!;

    // Dönem
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    // Dosya bilgileri
    public string FileName { get; private set; } = null!;
    public string FilePath { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string FileChecksum { get; private set; } = null!;
    public ReportFileFormat FileFormat { get; private set; } = null!;

    // Durum
    public ReportStatus Status { get; private set; } = null!;

    // İstatistikler
    public int RecordCount { get; private set; }
    public decimal? TotalAmount { get; private set; }

    // Tarihler
    public DateTime GeneratedAt { get; private set; }
    public DateTime? ValidatedAt { get; private set; }
    public string? ValidatedBy { get; private set; }

    // Hata
    public string? ErrorMessage { get; private set; }

    // Gönderim
    private readonly List<ReportSubmission> _submissions = new();
    public IReadOnlyCollection<ReportSubmission> Submissions => _submissions.AsReadOnly();

    private GeneratedReport() { }

    public static Result<GeneratedReport> Create(
        Guid reportDefinitionId,
        DateTime periodStart,
        DateTime periodEnd,
        ReportFileFormat fileFormat)
    {
        if (periodEnd < periodStart)
            return Result.Failure<GeneratedReport>("Dönem sonu, dönem başından önce olamaz");

        var report = new GeneratedReport
        {
            ReportNumber = GenerateReportNumber(),
            ReportDefinitionId = reportDefinitionId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            FileFormat = fileFormat,
            Status = ReportStatus.Pending,
            GeneratedAt = DateTime.UtcNow
        };

        return report;
    }

    /// <summary>
    /// Üretim başlat
    /// </summary>
    public Result StartGeneration()
    {
        if (Status != ReportStatus.Pending)
            return Result.Failure("Sadece bekleyen rapor üretilebilir");

        Status = ReportStatus.Generating;
        return Result.Success();
    }

    /// <summary>
    /// Üretim tamamla
    /// </summary>
    public Result CompleteGeneration(
        string fileName,
        string filePath,
        long fileSizeBytes,
        string checksum,
        int recordCount,
        decimal? totalAmount = null)
    {
        if (Status != ReportStatus.Generating)
            return Result.Failure("Sadece üretiliyor durumundaki rapor tamamlanabilir");

        FileName = fileName;
        FilePath = filePath;
        FileSizeBytes = fileSizeBytes;
        FileChecksum = checksum;
        RecordCount = recordCount;
        TotalAmount = totalAmount;
        Status = ReportStatus.Generated;

        return Result.Success();
    }

    /// <summary>
    /// Üretim başarısız
    /// </summary>
    public Result FailGeneration(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        return Result.Success();
    }

    /// <summary>
    /// Doğrula
    /// </summary>
    public Result Validate(string validatedBy)
    {
        if (!Status.CanValidate)
            return Result.Failure($"Bu durumda doğrulanamaz. Mevcut durum: {Status.DisplayName}");

        Status = ReportStatus.Validated;
        ValidatedAt = DateTime.UtcNow;
        ValidatedBy = validatedBy;

        return Result.Success();
    }

    /// <summary>
    /// Gönderim ekle
    /// </summary>
    public Result AddSubmission(ReportSubmission submission)
    {
        if (!Status.CanSubmit)
            return Result.Failure($"Bu durumda gönderilemez. Mevcut durum: {Status.DisplayName}");

        _submissions.Add(submission);
        Status = ReportStatus.Submitted;

        return Result.Success();
    }

    /// <summary>
    /// Kabul edildi
    /// </summary>
    public Result MarkAsAccepted()
    {
        if (Status != ReportStatus.Submitted)
            return Result.Failure("Sadece gönderilen rapor kabul edilebilir");

        Status = ReportStatus.Accepted;
        return Result.Success();
    }

    /// <summary>
    /// Reddedildi
    /// </summary>
    public Result MarkAsRejected(string reason)
    {
        if (Status != ReportStatus.Submitted)
            return Result.Failure("Sadece gönderilen rapor reddedilebilir");

        Status = ReportStatus.Rejected;
        ErrorMessage = reason;
        return Result.Success();
    }

    private static string GenerateReportNumber()
    {
        return $"RPT{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}