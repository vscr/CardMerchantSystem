using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Services;

namespace Accounting.Domain.Repositories;

/// <summary>
/// Hesap Bakiyesi Repository Interface
/// </summary>
public interface IAccountBalanceRepository
{
    Task<AccountBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AccountBalance?> GetByAccountAndPeriodAsync(Guid accountId, string periodCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountBalance>> GetByPeriodAsync(string periodCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountBalance>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountBalance>> GetNonZeroBalancesByPeriodAsync(string periodCode, CancellationToken cancellationToken = default);
    Task AddAsync(AccountBalance balance, CancellationToken cancellationToken = default);
    Task UpdateAsync(AccountBalance balance, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}