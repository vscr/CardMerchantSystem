using Fee.Domain.Entities;
using Fee.Domain.Enums;

namespace Fee.Domain.Repositories;

/// <summary>
/// Üye İşyeri Tarife Repository Interface
/// </summary>
public interface IMerchantTariffRepository
{
    Task<MerchantTariff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MerchantTariff?> GetActiveTariffAsync(string merchantId, FeeType feeType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantTariff>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantTariff>> GetByTariffIdAsync(Guid tariffId, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantTariff merchantTariff, CancellationToken cancellationToken = default);
    Task UpdateAsync(MerchantTariff merchantTariff, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}