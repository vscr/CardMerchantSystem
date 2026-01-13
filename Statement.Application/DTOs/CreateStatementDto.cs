namespace Statement.Application.DTOs;

/// <summary>
/// Ekstre Oluşturma DTO
/// </summary>
public class CreateStatementDto
{
    public string CardNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public DateTime DueDate { get; set; }
    public int StatementDay { get; set; }
    public decimal PreviousBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal InterestRate { get; set; }
    public decimal CashAdvanceInterestRate { get; set; }
}

/// <summary>
/// Toplu Ekstre Oluşturma DTO
/// </summary>
public class GenerateBatchStatementsDto
{
    public int StatementDay { get; set; }
    public DateTime? PeriodEndDate { get; set; }
}