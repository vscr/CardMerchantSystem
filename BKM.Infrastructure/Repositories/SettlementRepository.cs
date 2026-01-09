using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using BKM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BKM.Infrastructure.Repositories;

public class SettlementRepository : ISettlementRepository
{
    private readonly BKMDbContext _context;

    public SettlementRepository(BKMDbContext context)
    {
        _context = context;
    }

    public async Task<SettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Include(x => x.BankSummaries)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementBatch?> GetByDateAsync(string settlementDate, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Include(x => x.BankSummaries)
            .FirstOrDefaultAsync(x => x.SettlementDate == settlementDate, cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.IsCompleted)
            .OrderByDescending(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetByDateRangeAsync(string startDate, string endDate, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.SettlementDate.CompareTo(startDate) >= 0 && x.SettlementDate.CompareTo(endDate) <= 0)
            .OrderByDescending(x => x.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SettlementBatch batch, CancellationToken cancellationToken = default)
    {
        await _context.SettlementBatches.AddAsync(batch, cancellationToken);
    }

    public Task UpdateAsync(SettlementBatch batch, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}