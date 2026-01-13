using MerchantReport.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Entities;

/// <summary>
/// Rapor Talebi
/// </summary>
public class ReportRequest : AggregateRoot
{
    public string RequestNumber { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;
    public ReportType ReportType { get; private set; } = null!;
    public ReportFormat ReportFormat { get; private set; } = null!;
    public ReportStatus Status { get; private set; } = null!;

    // Dönem
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    // Dosya Bilgileri
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public long? FileSize { get; private set; }

    // Dağıtım
    public DeliveryMethod DeliveryMethod { get; private set; } = null!;
    public DateTime? DeliveredAt { get; private set; }
    public string? DeliveryDetails { get; private set; }

    // Hata Bilgisi
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    // İstatistikler
    public int TotalTransactions { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TotalCommission { get; private set; }
    public decimal NetAmount { get; private set; }

    // Talep eden
    public string? RequestedBy { get; private set; }
    public Guid? ReportConfigId { get; private set; }

    // EF Core için
    private ReportRequest() { }

    /// <summary>
    /// Yeni rapor talebi oluşturur
    /// </summary>
    public static Result<ReportRequest> Create(
        string merchantId,
        string merchantName,
        ReportType reportType,
        ReportFormat reportFormat,
        DeliveryMethod deliveryMethod,
        DateTime periodStart,
        DateTime periodEnd,
        string? requestedBy = null,
        Guid? reportConfigId = null)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<ReportRequest>("Üye işyeri ID boş olamaz");

        if (periodEnd < periodStart)
            return Result.Failure<ReportRequest>("Dönem bitiş tarihi başlangıçtan küçük olamaz");

        var request = new ReportRequest
        {
            RequestNumber = GenerateRequestNumber(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            ReportType = reportType,
            ReportFormat = reportFormat,
            DeliveryMethod = deliveryMethod,
            Status = ReportStatus.Pending,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            RequestedBy = requestedBy,
            ReportConfigId = reportConfigId,
            RetryCount = 0
        };

        return request;
    }

    /// <summary>
    /// Oluşturuluyor olarak işaretle
    /// </summary>
    public Result StartGenerating()
    {
        if (Status != ReportStatus.Pending && Status != ReportStatus.Failed)
            return Result.Failure("Rapor bu durumda başlatılamaz");

        Status = ReportStatus.Generating;
        return Result.Success();
    }

    /// <summary>
    /// Oluşturuldu olarak işaretle
    /// </summary>
    public Result MarkAsGenerated(
        string fileName,
        string filePath,
        long fileSize,
        int totalTransactions,
        decimal totalAmount,
        decimal totalCommission,
        decimal netAmount)
    {
        if (Status != ReportStatus.Generating)
            return Result.Failure("Rapor oluşturma durumunda değil");

        Status = ReportStatus.Generated;
        FileName = fileName;
        FilePath = filePath;
        FileSize = fileSize;
        TotalTransactions = totalTransactions;
        TotalAmount = totalAmount;
        TotalCommission = totalCommission;
        NetAmount = netAmount;

        return Result.Success();
    }

    /// <summary>
    /// Gönderiliyor olarak işaretle
    /// </summary>
    public Result StartDelivering()
    {
        if (Status != ReportStatus.Generated)
            return Result.Failure("Rapor önce oluşturulmalı");

        if (DeliveryMethod == DeliveryMethod.None)
        {
            Status = ReportStatus.Delivered;
            return Result.Success();
        }

        Status = ReportStatus.Delivering;
        return Result.Success();
    }

    /// <summary>
    /// Teslim edildi olarak işaretle
    /// </summary>
    public Result MarkAsDelivered(string? deliveryDetails = null)
    {
        if (Status != ReportStatus.Delivering && Status != ReportStatus.Generated)
            return Result.Failure("Rapor teslim edilebilir durumda değil");

        Status = ReportStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        DeliveryDetails = deliveryDetails;

        return Result.Success();
    }

    /// <summary>
    /// Başarısız olarak işaretle
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    /// <summary>
    /// İptal et
    /// </summary>
    public Result Cancel()
    {
        if (Status.IsFinal)
            return Result.Failure("Tamamlanmış rapor iptal edilemez");

        Status = ReportStatus.Cancelled;
        return Result.Success();
    }

    private static string GenerateRequestNumber()
    {
        return $"RPT{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
}