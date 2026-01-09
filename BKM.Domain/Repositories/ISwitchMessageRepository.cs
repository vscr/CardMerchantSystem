using BKM.Domain.Entities;
using BKM.Domain.Enums;

namespace BKM.Domain.Repositories;

/// <summary>
/// Switch Message Repository Interface
/// </summary>
public interface ISwitchMessageRepository
{
    Task<SwitchMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SwitchMessage?> GetBySTANAsync(string stan, CancellationToken cancellationToken = default);
    Task<SwitchMessage?> GetByRRNAsync(string rrn, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SwitchMessage>> GetByStatusAsync(SwitchMessageStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SwitchMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SwitchMessage>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task AddAsync(SwitchMessage message, CancellationToken cancellationToken = default);
    Task UpdateAsync(SwitchMessage message, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}