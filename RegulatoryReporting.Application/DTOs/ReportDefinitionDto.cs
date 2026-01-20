namespace RegulatoryReporting.Application.DTOs;

public class ReportDefinitionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Authority { get; set; } = null!;
    public string AuthorityDisplayName { get; set; } = null!;
    public string ReportType { get; set; } = null!;
    public string ReportTypeDisplayName { get; set; } = null!;
    public string FileFormat { get; set; } = null!;
    public string FileFormatDisplayName { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string FrequencyDisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? LastGeneratedAt { get; set; }
    public DateTime? NextScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportDefinitionDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int AuthorityId { get; set; }
    public int ReportTypeId { get; set; }
    public int FileFormatId { get; set; }
    public int FrequencyId { get; set; }
    public string? TemplateQuery { get; set; }
    public string? TemplateFilePath { get; set; }
}