using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;

namespace Transaction.Domain.Repositories;

/// <summary>
/// İşlem repository interface
/// </summary>
public interface ITransactionRepository
{
    Task<TransactionAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TransactionAggregate?> GetByReferenceNumberAsync(ReferenceNumber referenceNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetByCardNumberAsync(string cardNumberMasked, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetPendingSettlementAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default);

    Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default);

    Task AddAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default);

    Task UpdateAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}