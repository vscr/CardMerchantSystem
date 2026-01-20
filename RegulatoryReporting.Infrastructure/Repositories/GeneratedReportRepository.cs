using Microsoft.EntityFrameworkCore;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;
using RegulatoryReporting.Domain.Repositories;
using RegulatoryReporting.Infrastructure.Persistence;

namespace RegulatoryReporting.Infrastructure.Repositories;

public class GeneratedReportRepository : IGeneratedReportRepository
{
    private readonly RegulatoryReportingDbContext _context;

    public GeneratedReportRepository(RegulatoryReportingDbContext context)
    {
        _context = context;
    }

    public async Task<GeneratedReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GeneratedReport?> GetByIdWithSubmissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .Include(x => x.Submissions)
            .Include(x => x.Definition)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GeneratedReport?> GetByReportNumberAsync(string reportNumber, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .FirstOrDefaultAsync(x => x.ReportNumber == reportNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedReport>> GetByDefinitionIdAsync(Guid definitionId, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .Where(x => x.ReportDefinitionId == definitionId)
            .OrderByDescending(x => x.GeneratedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedReport>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.GeneratedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .Where(x => x.GeneratedAt >= startDate && x.GeneratedAt <= endDate)
            .OrderByDescending(x => x.GeneratedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedReport>> GetPendingSubmissionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedReports
            .Where(x => x.Status == ReportStatus.Generated || x.Status == ReportStatus.Validated)
            .OrderBy(x => x.GeneratedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GeneratedReport report, CancellationToken cancellationToken = default)
    {
        await _context.GeneratedReports.AddAsync(report, cancellationToken);
    }

    public void Update(GeneratedReport report)
    {
        _context.GeneratedReports.Update(report);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}