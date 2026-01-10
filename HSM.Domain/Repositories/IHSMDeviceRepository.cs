using HSM.Domain.Entities;

namespace HSM.Domain.Repositories;

/// <summary>
/// HSM Device Repository Interface
/// </summary>
public interface IHSMDeviceRepository
{
    Task<HSMDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HSMDevice?> GetByNameAsync(string deviceName, CancellationToken cancellationToken = default);
    Task<HSMDevice?> GetPrimaryDeviceAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMDevice>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(HSMDevice device, CancellationToken cancellationToken = default);
    Task UpdateAsync(HSMDevice device, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}