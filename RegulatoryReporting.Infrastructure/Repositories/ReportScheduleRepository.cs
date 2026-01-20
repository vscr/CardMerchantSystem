using Microsoft.EntityFrameworkCore;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;
using RegulatoryReporting.Infrastructure.Persistence;

namespace RegulatoryReporting.Infrastructure.Repositories;

public class ReportScheduleRepository : IReportScheduleRepository
{
    private readonly RegulatoryReportingDbContext _context;

    public ReportScheduleRepository(RegulatoryReportingDbContext context)
    {
        _context = context;
    }

    public async Task<ReportSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(x => x.Definition)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReportSchedule?> GetByDefinitionIdAsync(Guid definitionId, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(x => x.Definition)
            .FirstOrDefaultAsync(x => x.ReportDefinitionId == definitionId, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportSchedule>> GetEnabledAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(x => x.Definition)
            .Where(x => x.IsEnabled)
            .OrderBy(x => x.NextRunAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportSchedule>> GetDueSchedulesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(x => x.Definition)
            .Where(x => x.IsEnabled && x.NextRunAt.HasValue && x.NextRunAt <= asOfDate)
            .OrderBy(x => x.NextRunAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ReportSchedule schedule, CancellationToken cancellationToken = default)
    {
        await _context.ReportSchedules.AddAsync(schedule, cancellationToken);
    }

    public void Update(ReportSchedule schedule)
    {
        _context.ReportSchedules.Update(schedule);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}