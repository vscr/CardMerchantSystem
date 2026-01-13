namespace Accounting.Application.DTOs;

/// <summary>
/// Hesap Planı DTO
/// </summary>
public class ChartOfAccountDto
{
    public Guid Id { get; set; }
    public string AccountCode { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string? Description { get; set; }
    public string AccountType { get; set; } = null!;
    public string AccountTypeDisplayName { get; set; } = null!;
    public Guid? ParentAccountId { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public bool IsPostable { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Hesap Oluşturma DTO
/// </summary>
public class CreateChartOfAccountDto
{
    public string AccountCode { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string? Description { get; set; }
    public int AccountTypeId { get; set; }
    public Guid? ParentAccountId { get; set; }
    public int Level { get; set; }
    public bool IsPostable { get; set; } = true;
    public string? CurrencyCode { get; set; } = "TRY";
}