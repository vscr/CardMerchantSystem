namespace RegulatoryReporting.Application.DTOs;

public class ReportScheduleDto
{
    public Guid Id { get; set; }
    public Guid ReportDefinitionId { get; set; }
    public string ReportDefinitionName { get; set; } = null!;
    public int DayOfMonth { get; set; }
    public int? DayOfWeek { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public int ConsecutiveFailures { get; set; }
    public string? LastErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportScheduleDto
{
    public Guid ReportDefinitionId { get; set; }
    public int DayOfMonth { get; set; }
    public int? DayOfWeek { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}