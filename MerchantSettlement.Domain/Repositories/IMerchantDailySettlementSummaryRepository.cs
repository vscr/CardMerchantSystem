using MerchantSettlement.Domain.Entities;

namespace MerchantSettlement.Domain.Repositories;

public interface IMerchantDailySettlementSummaryRepository
{
    Task<MerchantDailySettlementSummary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantDailySettlementSummary?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantDailySettlementSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantDailySettlementSummary>> GetUnfinalizedAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MerchantDailySettlementSummary summary, CancellationToken cancellationToken = default);
    void Update(MerchantDailySettlementSummary summary);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}