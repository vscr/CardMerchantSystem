namespace Transaction.Application.DTOs;

/// <summary>
/// Limit bilgisi DTO
/// </summary>
public class LimitInfoDto
{
    public string CardNumberMasked { get; set; } = null!;
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal DailyUsed { get; set; }
    public decimal MonthlyUsed { get; set; }
    public decimal RemainingDailyLimit { get; set; }
    public decimal RemainingMonthlyLimit { get; set; }
    public string Currency { get; set; } = "TRY";
}