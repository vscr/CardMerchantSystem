using Statement.Domain.Entities;

namespace Statement.Domain.Repositories;

/// <summary>
/// Ekstre Kesim Ayarları Repository Interface
/// </summary>
public interface IStatementPeriodConfigRepository
{
    Task<StatementPeriodConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StatementPeriodConfig?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatementPeriodConfig>> GetByStatementDayAsync(int statementDay, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatementPeriodConfig>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StatementPeriodConfig config, CancellationToken cancellationToken = default);
    Task UpdateAsync(StatementPeriodConfig config, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}