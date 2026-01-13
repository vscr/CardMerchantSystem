using Statement.Domain.Entities;
using Statement.Domain.Repositories;
using Statement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Statement.Infrastructure.Repositories;

public class StatementNotificationRepository : IStatementNotificationRepository
{
    private readonly StatementDbContext _context;

    public StatementNotificationRepository(StatementDbContext context)
    {
        _context = context;
    }

    public async Task<StatementNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StatementNotifications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StatementNotification>> GetByStatementIdAsync(Guid statementId, CancellationToken cancellationToken = default)
    {
        return await _context.StatementNotifications
            .Where(x => x.StatementId == statementId)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StatementNotification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StatementNotifications
            .Where(x => !x.IsSent && x.RetryCount < 3)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StatementNotification>> GetFailedNotificationsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default)
    {
        return await _context.StatementNotifications
            .Where(x => !x.IsSent && x.RetryCount >= maxRetryCount)
            .OrderByDescending(x => x.RetryCount)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StatementNotification notification, CancellationToken cancellationToken = default)
    {
        await _context.StatementNotifications.AddAsync(notification, cancellationToken);
    }

    public Task UpdateAsync(StatementNotification notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}