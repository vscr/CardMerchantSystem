using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using BKM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BKM.Infrastructure.Repositories;

public class ClearingRepository : IClearingRepository
{
    private readonly BKMDbContext _context;

    public ClearingRepository(BKMDbContext context)
    {
        _context = context;
    }

    public async Task<ClearingRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ClearingRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ClearingRecord?> GetBySTANAsync(string stan, CancellationToken cancellationToken = default)
    {
        return await _context.ClearingRecords
            .FirstOrDefaultAsync(x => x.STAN == stan, cancellationToken);
    }

    public async Task<IReadOnlyList<ClearingRecord>> GetByClearingDateAsync(string clearingDate, CancellationToken cancellationToken = default)
    {
        return await _context.ClearingRecords
            .Where(x => x.ClearingDate == clearingDate)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClearingRecord>> GetUnsettledAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ClearingRecords
            .Where(x => !x.IsSettled)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClearingRecord>> GetByBankCodeAsync(string bankCode, bool isAcquirer, CancellationToken cancellationToken = default)
    {
        if (isAcquirer)
        {
            return await _context.ClearingRecords
                .Where(x => x.AcquirerBankCode == bankCode)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync(cancellationToken);
        }
        else
        {
            return await _context.ClearingRecords
                .Where(x => x.IssuerBankCode == bankCode)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task AddAsync(ClearingRecord record, CancellationToken cancellationToken = default)
    {
        await _context.ClearingRecords.AddAsync(record, cancellationToken);
    }

    public Task UpdateAsync(ClearingRecord record, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}