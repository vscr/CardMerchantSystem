using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Repositories;

public interface IMerchantPayoutRepository
{
    Task<MerchantPayout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantPayout?> GetByPayoutNumberAsync(string payoutNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantPayout>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantPayout>> GetByStatusAsync(PayoutStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantPayout>> GetScheduledPayoutsAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantPayout>> GetPendingPayoutsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantPayout>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantPayout payout, CancellationToken cancellationToken = default);
    void Update(MerchantPayout payout);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}