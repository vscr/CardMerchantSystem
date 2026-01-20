using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Domain.Repositories;

public interface IReportDefinitionRepository
{
    Task<ReportDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReportDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportDefinition>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportDefinition>> GetByAuthorityAsync(RegulatoryAuthority authority, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportDefinition>> GetDueForGenerationAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ReportDefinition definition, CancellationToken cancellationToken = default);
    void Update(ReportDefinition definition);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}