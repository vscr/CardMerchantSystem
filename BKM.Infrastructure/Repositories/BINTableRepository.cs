using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using BKM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BKM.Infrastructure.Repositories;

public class BINTableRepository : IBINTableRepository
{
    private readonly BKMDbContext _context;

    public BINTableRepository(BKMDbContext context)
    {
        _context = context;
    }

    public async Task<BINTable?> GetByBINAsync(string bin, CancellationToken cancellationToken = default)
    {
        return await _context.BINTables
            .FirstOrDefaultAsync(x => x.BIN == bin, cancellationToken);
    }

    public async Task<IReadOnlyList<BINTable>> GetByBankCodeAsync(string bankCode, CancellationToken cancellationToken = default)
    {
        return await _context.BINTables
            .Where(x => x.BankCode == bankCode && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BINTable>> GetByCardBrandAsync(string cardBrand, CancellationToken cancellationToken = default)
    {
        return await _context.BINTables
            .Where(x => x.CardBrand == cardBrand && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BINTable>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BINTables
            .Where(x => x.IsActive)
            .OrderBy(x => x.BIN)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BINTable binTable, CancellationToken cancellationToken = default)
    {
        await _context.BINTables.AddAsync(binTable, cancellationToken);
    }

    public Task UpdateAsync(BINTable binTable, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}