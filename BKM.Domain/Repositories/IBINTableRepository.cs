using BKM.Domain.Entities;
using BKM.Domain.Enums;
using BKM.Domain.Services;

namespace BKM.Domain.Repositories;

/// <summary>
/// BIN Table Repository Interface
/// </summary>
public interface IBINTableRepository
{
    Task<BINTable?> GetByBINAsync(string bin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BINTable>> GetByBankCodeAsync(string bankCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BINTable>> GetByCardBrandAsync(string cardBrand, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BINTable>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BINTable binTable, CancellationToken cancellationToken = default);
    Task UpdateAsync(BINTable binTable, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}