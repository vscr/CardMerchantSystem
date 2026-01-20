using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class MerchantPayoutRepository : IMerchantPayoutRepository
{
    private readonly MerchantSettlementDbContext _context;

    public MerchantPayoutRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantPayout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantPayout?> GetByPayoutNumberAsync(string payoutNumber, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .FirstOrDefaultAsync(x => x.PayoutNumber == payoutNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantPayout>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantPayout>> GetByStatusAsync(PayoutStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantPayout>> GetScheduledPayoutsAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;

        return await _context.MerchantPayouts
            .Where(x => x.Status == PayoutStatus.Scheduled && x.ScheduledDate.HasValue && x.ScheduledDate.Value.Date == targetDate)
            .OrderBy(x => x.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantPayout>> GetPendingPayoutsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .Where(x => x.Status == PayoutStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantPayout>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantPayouts
            .Where(x => x.PeriodStart >= startDate && x.PeriodEnd <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantPayout payout, CancellationToken cancellationToken = default)
    {
        await _context.MerchantPayouts.AddAsync(payout, cancellationToken);
    }

    public void Update(MerchantPayout payout)
    {
        _context.MerchantPayouts.Update(payout);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}