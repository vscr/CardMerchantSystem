namespace Statement.Application.DTOs;

/// <summary>
/// Kesim Ayarları DTO
/// </summary>
public class StatementPeriodConfigDto
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } = null!;
    public int StatementDay { get; set; }
    public int PaymentDueDays { get; set; }
    public decimal InterestRate { get; set; }
    public decimal CashAdvanceInterestRate { get; set; }
    public decimal MinimumPaymentRate { get; set; }
    public decimal MinimumPaymentAmount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Kesim Ayarı Oluşturma DTO
/// </summary>
public class CreateStatementPeriodConfigDto
{
    public string CardNumber { get; set; } = null!;
    public int StatementDay { get; set; }
    public int PaymentDueDays { get; set; } = 10;
    public decimal InterestRate { get; set; } = 42.0m;
    public decimal CashAdvanceInterestRate { get; set; } = 54.0m;
    public decimal MinimumPaymentRate { get; set; } = 20.0m;
    public decimal MinimumPaymentAmount { get; set; } = 100.0m;
}