using MerchantSettlement.Domain.Entities;

namespace MerchantSettlement.Domain.Repositories;

public interface IDailySettlementSummaryRepository
{
    Task<DailySettlementSummary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DailySettlementSummary?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DailySettlementSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DailySettlementSummary>> GetUnfinalizedAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DailySettlementSummary summary, CancellationToken cancellationToken = default);
    void Update(DailySettlementSummary summary);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}