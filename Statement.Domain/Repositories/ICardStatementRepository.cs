using Statement.Domain.Entities;
using Statement.Domain.Enums;

namespace Statement.Domain.Repositories;

/// <summary>
/// Kart Ekstre Repository Interface
/// </summary>
public interface ICardStatementRepository
{
    Task<CardStatement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardStatement?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardStatement?> GetByStatementNumberAsync(string statementNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardStatement>> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task<CardStatement?> GetLatestByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardStatement>> GetByStatusAsync(StatementStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardStatement>> GetOverdueStatementsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardStatement>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardStatement>> GetPendingForNotificationAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CardStatement statement, CancellationToken cancellationToken = default);
    Task UpdateAsync(CardStatement statement, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}