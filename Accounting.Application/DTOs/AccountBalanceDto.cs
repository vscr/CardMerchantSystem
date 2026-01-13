namespace Accounting.Application.DTOs;

/// <summary>
/// Hesap Bakiyesi DTO
/// </summary>
public class AccountBalanceDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string PeriodCode { get; set; } = null!;
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public decimal PeriodDebit { get; set; }
    public decimal PeriodCredit { get; set; }
    public decimal ClosingDebit { get; set; }
    public decimal ClosingCredit { get; set; }
    public decimal NetBalance { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Mizan DTO
/// </summary>
public class TrialBalanceDto
{
    public string PeriodCode { get; set; } = null!;
    public string PeriodName { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
    public decimal TotalOpeningDebit { get; set; }
    public decimal TotalOpeningCredit { get; set; }
    public decimal TotalPeriodDebit { get; set; }
    public decimal TotalPeriodCredit { get; set; }
    public decimal TotalClosingDebit { get; set; }
    public decimal TotalClosingCredit { get; set; }
    public List<AccountBalanceDto> Accounts { get; set; } = new();
}