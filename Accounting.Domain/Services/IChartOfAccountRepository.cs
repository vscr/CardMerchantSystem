using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Repositories;

/// <summary>
/// Hesap Planı Repository Interface
/// </summary>
public interface IChartOfAccountRepository
{
    Task<ChartOfAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChartOfAccount?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetByTypeAsync(AccountType accountType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetPostableAccountsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ChartOfAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(ChartOfAccount account, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}