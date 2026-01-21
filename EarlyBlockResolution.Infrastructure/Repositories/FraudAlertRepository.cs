using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using EarlyBlockResolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyBlockResolution.Infrastructure.Repositories;

public class FraudAlertRepository : IFraudAlertRepository
{
    private readonly EarlyBlockResolutionDbContext _context;

    public FraudAlertRepository(EarlyBlockResolutionDbContext context)
    {
        _context = context;
    }

    public async Task<FraudAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<FraudAlert?> GetByAlertNumberAsync(string alertNumber, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .FirstOrDefaultAsync(x => x.AlertNumber == alertNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<FraudAlert>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .Where(x => x.CardId == cardId)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FraudAlert>> GetUnprocessedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .Where(x => !x.IsProcessed)
            .OrderBy(x => x.Severity)
            .ThenBy(x => x.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FraudAlert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .Where(x => x.Severity == severity)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FraudAlert>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .Where(x => x.DetectedAt >= startDate && x.DetectedAt <= endDate)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FraudAlert alert, CancellationToken cancellationToken = default)
    {
        await _context.FraudAlerts.AddAsync(alert, cancellationToken);
    }

    public void Update(FraudAlert alert)
    {
        _context.FraudAlerts.Update(alert);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}