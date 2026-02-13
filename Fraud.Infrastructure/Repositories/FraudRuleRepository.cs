using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class FraudRuleRepository : IFraudRuleRepository
{
    private readonly FraudDbContext _context;

    public FraudRuleRepository(FraudDbContext context) => _context = context;

    public async Task<FraudRule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.FraudRules.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<FraudRule?> GetByIdWithConditionsAsync(Guid id, CancellationToken ct = default)
        => await _context.FraudRules
            .Include(x => x.Conditions)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<FraudRule?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _context.FraudRules
            .Include(x => x.Conditions)
            .FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task<List<FraudRule>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.FraudRules
            .AsNoTracking()
            .Include(x => x.Conditions.OrderBy(c => c.OrderIndex))
            .Where(x => x.IsActive)
            .ToListAsync(ct);

    public async Task AddAsync(FraudRule rule, CancellationToken ct = default)
    {
        await _context.FraudRules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FraudRule rule, CancellationToken ct = default)
    {
        _context.FraudRules.Update(rule);
        await _context.SaveChangesAsync(ct);
    }
}