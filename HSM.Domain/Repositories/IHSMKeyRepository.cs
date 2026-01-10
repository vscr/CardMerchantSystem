using HSM.Domain.Entities;
using HSM.Domain.Enums;

namespace HSM.Domain.Repositories;

/// <summary>
/// HSM Key Repository Interface
/// </summary>
public interface IHSMKeyRepository
{
    Task<HSMKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HSMKey?> GetByNameAsync(string keyName, CancellationToken cancellationToken = default);
    Task<HSMKey?> GetByIndexAsync(string keyIndex, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMKey>> GetByTypeAsync(HSMKeyType keyType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMKey>> GetByDeviceIdAsync(Guid hsmDeviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMKey>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(HSMKey key, CancellationToken cancellationToken = default);
    Task UpdateAsync(HSMKey key, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}