namespace MerchantReport.Application.DTOs;

/// <summary>
/// Rapor Talebi DTO
/// </summary>
public class ReportRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public string ReportType { get; set; } = null!;
    public string ReportTypeDisplayName { get; set; } = null!;
    public string ReportFormat { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string DeliveryMethod { get; set; } = null!;
    public DateTime? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal NetAmount { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Rapor Talebi Özet DTO
/// </summary>
public class ReportRequestSummaryDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public string ReportTypeDisplayName { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Rapor Talebi Oluşturma DTO
/// </summary>
public class CreateReportRequestDto
{
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public int ReportTypeId { get; set; }
    public int ReportFormatId { get; set; }
    public int DeliveryMethodId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}