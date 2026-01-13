using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Services;

namespace MerchantReport.Domain.Repositories;

/// <summary>
/// Üye İşyeri Ekstresi Repository Interface
/// </summary>
public interface IMerchantStatementRepository
{
    Task<MerchantStatement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantStatement?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantStatement?> GetByStatementNumberAsync(string statementNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantStatement>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<MerchantStatement?> GetLatestByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantStatement>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantStatement statement, CancellationToken cancellationToken = default);
    Task UpdateAsync(MerchantStatement statement, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}