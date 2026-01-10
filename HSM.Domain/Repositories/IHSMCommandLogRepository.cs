using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Services;

namespace HSM.Domain.Repositories;

/// <summary>
/// HSM Command Log Repository Interface
/// </summary>
public interface IHSMCommandLogRepository
{
    Task<HSMCommandLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMCommandLog>> GetByDeviceIdAsync(Guid hsmDeviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMCommandLog>> GetByCommandTypeAsync(HSMCommandType commandType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMCommandLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HSMCommandLog>> GetFailedCommandsAsync(DateTime since, CancellationToken cancellationToken = default);
    Task AddAsync(HSMCommandLog log, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}