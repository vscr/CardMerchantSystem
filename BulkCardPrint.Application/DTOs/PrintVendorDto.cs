namespace BulkCardPrint.Application.DTOs;

public class PrintVendorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public string? ApiEndpoint { get; set; }
    public string? FtpHost { get; set; }
    public string PreferredFileFormat { get; set; } = null!;
    public string PreferredFileFormatDisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public int DailyCapacity { get; set; }
    public int CurrentDailyLoad { get; set; }
    public int RemainingCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePrintVendorDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public int FileFormatId { get; set; }
    public int DailyCapacity { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? FtpHost { get; set; }
    public string? FtpUsername { get; set; }
    public string? FtpPath { get; set; }
}