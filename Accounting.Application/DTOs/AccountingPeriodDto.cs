namespace Accounting.Application.DTOs;

/// <summary>
/// Muhasebe Dönemi DTO
/// </summary>
public class AccountingPeriodDto
{
    public Guid Id { get; set; }
    public string PeriodCode { get; set; } = null!;
    public string PeriodName { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Dönem Oluşturma DTO
/// </summary>
public class CreateAccountingPeriodDto
{
    public int Year { get; set; }
    public int Month { get; set; }
}