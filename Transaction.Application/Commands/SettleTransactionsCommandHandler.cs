using Transaction.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Commands;

public class SettleTransactionsCommandHandler
    : IRequestHandler<SettleTransactionsCommand, Result<SettlementResultDto>>
{
    private readonly ITransactionRepository _repository;

    public SettleTransactionsCommandHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SettlementResultDto>> Handle(
        SettleTransactionsCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Takas bekleyen işlemleri getir
        var pendingTransactions = await _repository.GetPendingSettlementAsync(cancellationToken);

        if (!pendingTransactions.Any())
            return Result.Failure<SettlementResultDto>("Takas edilecek işlem bulunamadı");

        var successCount = 0;
        var failCount = 0;
        var totalAmount = 0m;

        // 2. Her işlemi takas et
        foreach (var transaction in pendingTransactions)
        {
            var settleResult = transaction.Settle(request.BatchNumber);

            if (settleResult.IsSuccess)
            {
                successCount++;
                totalAmount += transaction.Amount.Amount;
                await _repository.UpdateAsync(transaction, cancellationToken);
            }
            else
            {
                failCount++;
            }
        }

        // 3. Kaydet
        await _repository.SaveChangesAsync(cancellationToken);

        return new SettlementResultDto
        {
            BatchNumber = request.BatchNumber,
            TotalTransactions = pendingTransactions.Count,
            SuccessfulSettlements = successCount,
            FailedSettlements = failCount,
            TotalAmount = totalAmount,
            SettledAt = DateTime.UtcNow
        };
    }
}