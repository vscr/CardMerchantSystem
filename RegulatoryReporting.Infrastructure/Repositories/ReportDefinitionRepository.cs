using Microsoft.EntityFrameworkCore;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;
using RegulatoryReporting.Domain.Repositories;
using RegulatoryReporting.Infrastructure.Persistence;

namespace RegulatoryReporting.Infrastructure.Repositories;

public class ReportDefinitionRepository : IReportDefinitionRepository
{
    private readonly RegulatoryReportingDbContext _context;

    public ReportDefinitionRepository(RegulatoryReportingDbContext context)
    {
        _context = context;
    }

    public async Task<ReportDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportDefinitions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReportDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.ReportDefinitions
            .FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<ReportDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportDefinitions
            .OrderBy(x => x.Authority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportDefinition>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportDefinitions
            .Where(x => x.IsActive)
            .OrderBy(x => x.Authority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportDefinition>> GetByAuthorityAsync(RegulatoryAuthority authority, CancellationToken cancellationToken = default)
    {
        return await _context.ReportDefinitions
            .Where(x => x.Authority == authority)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportDefinition>> GetDueForGenerationAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.ReportDefinitions
            .Where(x => x.IsActive && x.NextScheduledAt.HasValue && x.NextScheduledAt <= now)
            .OrderBy(x => x.NextScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ReportDefinition definition, CancellationToken cancellationToken = default)
    {
        await _context.ReportDefinitions.AddAsync(definition, cancellationToken);
    }

    public void Update(ReportDefinition definition)
    {
        _context.ReportDefinitions.Update(definition);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}