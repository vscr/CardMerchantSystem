using CardMerchantSystem.Shared.Kernel;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;

namespace Merchant.Domain.Repositories;

/// <summary>
/// Üye işyeri repository interface
/// </summary>
public interface IMerchantRepository
{
    Task<MerchantAggregate?> GetByIdWithRetryAsync(Guid id, CancellationToken cancellationToken);
    Task<MerchantAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MerchantAggregate?> GetByIdWithTerminalsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MerchantAggregate?> GetByMerchantCodeAsync(MerchantCode code, CancellationToken cancellationToken = default);

    Task<MerchantAggregate?> GetByTaxNumberAsync(TaxNumber taxNumber, CancellationToken cancellationToken = default);

    Task<bool> ExistsByTaxNumberAsync(TaxNumber taxNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MerchantAggregate>> GetByStatusAsync(MerchantStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MerchantAggregate>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sayfalı üye işyeri listesi getirir
    /// </summary>
    Task<(IReadOnlyList<MerchantAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        MerchantStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(MerchantAggregate merchant, CancellationToken cancellationToken = default);

    Task UpdateAsync(MerchantAggregate merchant, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}