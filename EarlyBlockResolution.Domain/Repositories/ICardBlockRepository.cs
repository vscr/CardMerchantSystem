using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;

namespace EarlyBlockResolution.Domain.Repositories;

public interface ICardBlockRepository
{
    Task<CardBlock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardBlock?> GetByIdWithVerificationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardBlock?> GetByBlockNumberAsync(string blockNumber, CancellationToken cancellationToken = default);
    Task<CardBlock?> GetActiveBlockByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardBlock>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardBlock>> GetByStatusAsync(BlockStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardBlock>> GetPendingVerificationAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardBlock>> GetExpiredBlocksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardBlock>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(CardBlock block, CancellationToken cancellationToken = default);
    void Update(CardBlock block);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}