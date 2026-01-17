using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class DailySettlementSummaryRepository : IDailySettlementSummaryRepository
{
    private readonly MerchantSettlementDbContext _context;

    public DailySettlementSummaryRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<DailySettlementSummary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DailySettlementSummaries
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DailySettlementSummary?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;

        return await _context.DailySettlementSummaries
            .FirstOrDefaultAsync(x => x.SettlementDate == targetDate, cancellationToken);
    }

    public async Task<IReadOnlyList<DailySettlementSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.DailySettlementSummaries
            .Where(x => x.SettlementDate >= startDate.Date && x.SettlementDate <= endDate.Date)
            .OrderByDescending(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DailySettlementSummary>> GetUnfinalizedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DailySettlementSummaries
            .Where(x => !x.IsFinalized)
            .OrderBy(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DailySettlementSummary summary, CancellationToken cancellationToken = default)
    {
        await _context.DailySettlementSummaries.AddAsync(summary, cancellationToken);
    }

    public void Update(DailySettlementSummary summary)
    {
        _context.DailySettlementSummaries.Update(summary);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}