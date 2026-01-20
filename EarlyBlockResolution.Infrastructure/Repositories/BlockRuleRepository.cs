using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using EarlyBlockResolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyBlockResolution.Infrastructure.Repositories;

public class BlockRuleRepository : IBlockRuleRepository
{
    private readonly EarlyBlockResolutionDbContext _context;

    public BlockRuleRepository(EarlyBlockResolutionDbContext context)
    {
        _context = context;
    }

    public async Task<BlockRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<BlockRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<BlockRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockRule>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .Where(x => x.IsActive)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockRule>> GetByReasonAsync(BlockReason reason, CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .Where(x => x.TriggerReason == reason && x.IsActive)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockRule>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
    {
        return await _context.BlockRules
            .Where(x => x.Severity == severity && x.IsActive)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BlockRule rule, CancellationToken cancellationToken = default)
    {
        await _context.BlockRules.AddAsync(rule, cancellationToken);
    }

    public void Update(BlockRule rule)
    {
        _context.BlockRules.Update(rule);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}