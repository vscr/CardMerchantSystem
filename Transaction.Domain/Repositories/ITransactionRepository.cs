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

    /// <summary>
    /// Sayfalı işlem listesi getirir
    /// </summary>
    Task<(IReadOnlyList<TransactionAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TransactionStatus? status = null,
        TransactionType? transactionType = null,
        Guid? merchantId = null,
        string? cardNumberMasked = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);

    Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default);

    Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Orijinal işleme ait tüm iade işlemlerini getirir
    /// </summary>
    Task<IReadOnlyList<TransactionAggregate>> GetRefundsByOriginalTransactionIdAsync(
        Guid originalTransactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Orijinal işlem için toplam iade edilmiş tutarı hesaplar
    /// Sadece Approved ve Settled durumundaki iadeler dahil edilir
    /// </summary>
    Task<decimal> GetTotalRefundedAmountAsync(
        Guid originalTransactionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default);

    Task UpdateAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}