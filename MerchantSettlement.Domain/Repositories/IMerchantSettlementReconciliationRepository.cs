using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Repositories;

public interface IMerchantSettlementReconciliationRepository
{
    Task<MerchantReconciliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantReconciliation?> GetByIdWithMismatchesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber, CancellationToken cancellationToken = default);
    Task<MerchantReconciliation?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReconciliation>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReconciliation>> GetByStatusAsync(ReconciliationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReconciliation>> GetUnresolvedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReconciliation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantReconciliation reconciliation, CancellationToken cancellationToken = default);
    void Update(MerchantReconciliation reconciliation);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}