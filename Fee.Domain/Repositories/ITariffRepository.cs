using Fee.Domain.Entities;
using Fee.Domain.Enums;

namespace Fee.Domain.Repositories;

/// <summary>
/// Tarife Repository Interface
/// </summary>
public interface ITariffRepository
{
    Task<Tariff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tariff?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tariff?> GetByCodeAsync(string tariffCode, CancellationToken cancellationToken = default);
    Task<Tariff?> GetDefaultTariffAsync(FeeType feeType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tariff>> GetActiveByFeeTypeAsync(FeeType feeType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tariff>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Tariff tariff, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}