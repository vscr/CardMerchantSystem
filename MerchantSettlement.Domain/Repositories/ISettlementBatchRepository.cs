using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Repositories;

public interface ISettlementBatchRepository
{
    Task<SettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetByStatusAsync(SettlementStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementBatch>> GetBySettlementDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task AddAsync(SettlementBatch batch, CancellationToken cancellationToken = default);
    void Update(SettlementBatch batch);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}