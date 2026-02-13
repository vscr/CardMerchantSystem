using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class FraudScenarioRepository : IFraudScenarioRepository
{
    private readonly FraudDbContext _context;

    public FraudScenarioRepository(FraudDbContext context) => _context = context;

    public async Task<FraudScenario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.FraudScenarios
            .Include(x => x.Rule).ThenInclude(r => r.Conditions)
            .Include(x => x.FilterRule)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<FraudScenario>> GetAllAsync(CancellationToken ct = default)
        => await _context.FraudScenarios
            .AsNoTracking()
            .OrderBy(x => x.RunOrder)
            .ToListAsync(ct);

    public async Task<List<FraudScenario>> GetActiveByModeAsync(FraudCheckMode mode, CancellationToken ct = default)
        => await _context.FraudScenarios
            .AsNoTracking()
            .Include(x => x.Rule).ThenInclude(r => r.Conditions)
            .Include(x => x.FilterRule).ThenInclude(r => r!.Conditions)
            .Where(x => x.IsActive && (x.CheckMode == mode || x.CheckMode == FraudCheckMode.Both))
            .OrderBy(x => x.RunOrder)
            .ToListAsync(ct);

    public async Task<int> GetNextScenarioNoAsync(CancellationToken ct = default)
    {
        var max = await _context.FraudScenarios.MaxAsync(x => (int?)x.ScenarioNo, ct);
        return (max ?? 0) + 1;
    }

    public async Task AddAsync(FraudScenario scenario, CancellationToken ct = default)
    {
        await _context.FraudScenarios.AddAsync(scenario, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FraudScenario scenario, CancellationToken ct = default)
    {
        _context.FraudScenarios.Update(scenario);
        await _context.SaveChangesAsync(ct);
    }
}