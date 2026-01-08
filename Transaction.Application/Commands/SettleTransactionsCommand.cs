using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Commands;

/// <summary>
/// Takas komutu (Günsonu)
/// </summary>
public record SettleTransactionsCommand(string BatchNumber) : IRequest<Result<SettlementResultDto>>;

public class SettlementResultDto
{
    public string BatchNumber { get; set; } = null!;
    public int TotalTransactions { get; set; }
    public int SuccessfulSettlements { get; set; }
    public int FailedSettlements { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SettledAt { get; set; }
}