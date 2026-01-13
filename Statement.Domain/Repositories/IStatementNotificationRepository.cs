using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Statement.Domain.Services;

namespace Statement.Domain.Repositories;

/// <summary>
/// Ekstre Bildirim Repository Interface
/// </summary>
public interface IStatementNotificationRepository
{
    Task<StatementNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatementNotification>> GetByStatementIdAsync(Guid statementId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatementNotification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatementNotification>> GetFailedNotificationsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default);
    Task AddAsync(StatementNotification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(StatementNotification notification, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}