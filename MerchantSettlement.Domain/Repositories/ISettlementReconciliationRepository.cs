using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Repositories;

public interface ISettlementReconciliationRepository
{
    Task<SettlementReconciliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementReconciliation?> GetByIdWithMismatchesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SettlementReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber, CancellationToken cancellationToken = default);
    Task<SettlementReconciliation?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementReconciliation>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementReconciliation>> GetByStatusAsync(ReconciliationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementReconciliation>> GetUnresolvedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementReconciliation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(SettlementReconciliation reconciliation, CancellationToken cancellationToken = default);
    void Update(SettlementReconciliation reconciliation);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}