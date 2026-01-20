using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Repositories;

public interface IMerchantSettlementBatchRepository
{
    Task<MerchantSettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantSettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantSettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantSettlementBatch>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantSettlementBatch>> GetByStatusAsync(SettlementStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantSettlementBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantSettlementBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantSettlementBatch>> GetBySettlementDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantSettlementBatch batch, CancellationToken cancellationToken = default);
    void Update(MerchantSettlementBatch batch);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}