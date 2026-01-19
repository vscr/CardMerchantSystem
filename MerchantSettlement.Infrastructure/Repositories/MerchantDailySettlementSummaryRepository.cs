using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class MerchantDailySettlementSummaryRepository : IMerchantDailySettlementSummaryRepository
{
    private readonly MerchantSettlementDbContext _context;

    public MerchantDailySettlementSummaryRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantDailySettlementSummary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantDailySettlementSummaries
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantDailySettlementSummary?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;

        return await _context.MerchantDailySettlementSummaries
            .FirstOrDefaultAsync(x => x.SettlementDate == targetDate, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantDailySettlementSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantDailySettlementSummaries
            .Where(x => x.SettlementDate >= startDate.Date && x.SettlementDate <= endDate.Date)
            .OrderByDescending(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantDailySettlementSummary>> GetUnfinalizedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MerchantDailySettlementSummaries
            .Where(x => !x.IsFinalized)
            .OrderBy(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantDailySettlementSummary summary, CancellationToken cancellationToken = default)
    {
        await _context.MerchantDailySettlementSummaries.AddAsync(summary, cancellationToken);
    }

    public void Update(MerchantDailySettlementSummary summary)
    {
        _context.MerchantDailySettlementSummaries.Update(summary);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}