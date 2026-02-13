using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class HitScenarioRepository : IHitScenarioRepository
{
    private readonly FraudDbContext _context;

    public HitScenarioRepository(FraudDbContext context) => _context = context;

    public async Task<List<HitScenario>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct = default)
        => await _context.HitScenarios
            .AsNoTracking()
            .Where(x => x.TransactionId == transactionId)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync(ct);

    public async Task<List<HitScenario>> GetByCardNoAsync(string maskedCardNo, int days = 30, CancellationToken ct = default)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        return await _context.HitScenarios
            .AsNoTracking()
            .Where(x => x.MaskedCardNo == maskedCardNo && x.DetectedAt >= from)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByScenarioIdAsync(Guid scenarioId, DateTime from, CancellationToken ct = default)
        => await _context.HitScenarios
            .CountAsync(x => x.FraudScenarioId == scenarioId && x.DetectedAt >= from, ct);

    public async Task AddAsync(HitScenario hitScenario, CancellationToken ct = default)
    {
        await _context.HitScenarios.AddAsync(hitScenario, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(List<HitScenario> hitScenarios, CancellationToken ct = default)
    {
        await _context.HitScenarios.AddRangeAsync(hitScenarios, ct);
        await _context.SaveChangesAsync(ct);
    }
}