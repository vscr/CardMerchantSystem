using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Repositories;

/// <summary>
/// Muhasebe Dönemi Repository Interface
/// </summary>
public interface IAccountingPeriodRepository
{
    Task<AccountingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AccountingPeriod?> GetByCodeAsync(string periodCode, CancellationToken cancellationToken = default);
    Task<AccountingPeriod?> GetCurrentPeriodAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountingPeriod>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountingPeriod>> GetByStatusAsync(PeriodStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountingPeriod>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AccountingPeriod period, CancellationToken cancellationToken = default);
    Task UpdateAsync(AccountingPeriod period, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}