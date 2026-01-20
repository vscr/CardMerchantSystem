using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Repositories;

public interface IBlockRuleRepository
{
    Task<BlockRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BlockRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlockRule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlockRule>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlockRule>> GetByReasonAsync(BlockReason reason, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlockRule>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);
    Task AddAsync(BlockRule rule, CancellationToken cancellationToken = default);
    void Update(BlockRule rule);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}