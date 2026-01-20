using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Repositories;

public interface IFraudAlertRepository
{
    Task<FraudAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FraudAlert?> GetByAlertNumberAsync(string alertNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FraudAlert>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FraudAlert>> GetUnprocessedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FraudAlert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FraudAlert>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(FraudAlert alert, CancellationToken cancellationToken = default);
    void Update(FraudAlert alert);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}