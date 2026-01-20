using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Domain.Repositories;

public interface IGeneratedReportRepository
{
    Task<GeneratedReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneratedReport?> GetByIdWithSubmissionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneratedReport?> GetByReportNumberAsync(string reportNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneratedReport>> GetByDefinitionIdAsync(Guid definitionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneratedReport>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneratedReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneratedReport>> GetPendingSubmissionAsync(CancellationToken cancellationToken = default);
    Task AddAsync(GeneratedReport report, CancellationToken cancellationToken = default);
    void Update(GeneratedReport report);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}