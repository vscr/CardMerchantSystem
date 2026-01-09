using BKM.Domain.Entities;

namespace BKM.Domain.Repositories;

/// <summary>
/// Settlement Repository Interface
/// </summary>
public interface ISettlementRepository
{
    Task<SettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementBatch?> GetByDateAsync(string settlementDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetCompletedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetByDateRangeAsync(string startDate, string endDate, CancellationToken cancellationToken = default);
    Task AddAsync(SettlementBatch batch, CancellationToken cancellationToken = default);
    Task UpdateAsync(SettlementBatch batch, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}