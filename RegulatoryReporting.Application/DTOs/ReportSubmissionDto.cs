namespace RegulatoryReporting.Application.DTOs;

public class ReportSubmissionDto
{
    public Guid Id { get; set; }
    public Guid GeneratedReportId { get; set; }
    public string SubmissionMethod { get; set; } = null!;
    public string? SubmissionReference { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string SubmittedBy { get; set; } = null!;
    public bool IsSuccessful { get; set; }
    public string? ResponseMessage { get; set; }
    public string? ErrorDetails { get; set; }
}

public class SubmitReportDto
{
    public Guid GeneratedReportId { get; set; }
    public string SubmissionMethod { get; set; } = null!;
}