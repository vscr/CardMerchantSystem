using RegulatoryReporting.Domain.Entities;

namespace RegulatoryReporting.Domain.Repositories;

public interface IReportScheduleRepository
{
    Task<ReportSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReportSchedule?> GetByDefinitionIdAsync(Guid definitionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportSchedule>> GetEnabledAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportSchedule>> GetDueSchedulesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task AddAsync(ReportSchedule schedule, CancellationToken cancellationToken = default);
    void Update(ReportSchedule schedule);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}