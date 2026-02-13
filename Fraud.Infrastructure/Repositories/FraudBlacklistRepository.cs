using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class FraudBlacklistRepository : IFraudBlacklistRepository
{
    private readonly FraudDbContext _context;

    public FraudBlacklistRepository(FraudDbContext context) => _context = context;

    public async Task<FraudBlacklist?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.FraudBlacklists.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<FraudBlacklist>> GetByListTypeAsync(string listType, CancellationToken ct = default)
        => await _context.FraudBlacklists
            .AsNoTracking()
            .Where(x => x.ListType == listType)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(string listType, string value, CancellationToken ct = default)
        => await _context.FraudBlacklists
            .AnyAsync(x => x.ListType == listType && x.Value == value, ct);

    public async Task<List<FraudBlacklist>> GetActiveBlacklistAsync(string listType, CancellationToken ct = default)
        => await _context.FraudBlacklists
            .AsNoTracking()
            .Where(x => x.ListType == listType && x.IsBlacklist && x.IsActive)
            .ToListAsync(ct);

    public async Task<List<FraudBlacklist>> GetActiveWhitelistAsync(string listType, CancellationToken ct = default)
        => await _context.FraudBlacklists
            .AsNoTracking()
            .Where(x => x.ListType == listType && !x.IsBlacklist && x.IsActive)
            .ToListAsync(ct);

    public async Task AddAsync(FraudBlacklist entry, CancellationToken ct = default)
    {
        await _context.FraudBlacklists.AddAsync(entry, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FraudBlacklist entry, CancellationToken ct = default)
    {
        _context.FraudBlacklists.Update(entry);
        await _context.SaveChangesAsync(ct);
    }
}