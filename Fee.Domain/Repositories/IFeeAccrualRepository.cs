using Fee.Domain.Entities;
using Fee.Domain.Enums;

namespace Fee.Domain.Repositories;

/// <summary>
/// Tahakkuk Repository Interface
/// </summary>
public interface IFeeAccrualRepository
{
    Task<FeeAccrual?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FeeAccrual?> GetByAccrualNumberAsync(string accrualNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeAccrual>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeAccrual>> GetByStatusAsync(AccrualStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeAccrual>> GetPendingByDueDateAsync(DateTime dueDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeAccrual>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeAccrual>> GetByPeriodAsync(string periodStart, string periodEnd, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalUnpaidByMerchantAsync(string merchantId, CancellationToken cancellationToken = default);
    Task AddAsync(FeeAccrual accrual, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeAccrual accrual, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}