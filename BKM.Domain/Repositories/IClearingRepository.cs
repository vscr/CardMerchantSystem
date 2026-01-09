using BKM.Domain.Entities;

namespace BKM.Domain.Repositories;

/// <summary>
/// Clearing Repository Interface
/// </summary>
public interface IClearingRepository
{
    Task<ClearingRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClearingRecord?> GetBySTANAsync(string stan, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClearingRecord>> GetByClearingDateAsync(string clearingDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClearingRecord>> GetUnsettledAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClearingRecord>> GetByBankCodeAsync(string bankCode, bool isAcquirer, CancellationToken cancellationToken = default);
    Task AddAsync(ClearingRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(ClearingRecord record, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}