namespace RegulatoryReporting.Application.DTOs;

public class GeneratedReportDto
{
    public Guid Id { get; set; }
    public string ReportNumber { get; set; } = null!;
    public Guid ReportDefinitionId { get; set; }
    public string ReportDefinitionName { get; set; } = null!;
    public string AuthorityDisplayName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public string FileFormat { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public int RecordCount { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string? ValidatedBy { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GeneratedReportWithSubmissionsDto : GeneratedReportDto
{
    public List<ReportSubmissionDto> Submissions { get; set; } = new();
}

public class GenerateReportDto
{
    public Guid ReportDefinitionId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}