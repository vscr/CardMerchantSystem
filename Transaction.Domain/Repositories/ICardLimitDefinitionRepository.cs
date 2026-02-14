using Transaction.Domain.Entities;

namespace Transaction.Domain.Repositories;

public interface ICardLimitDefinitionRepository
{
    Task<CardLimitDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CardLimitDefinition?> GetDefaultAsync(CancellationToken ct = default);
    Task<CardLimitDefinition?> GetByCardNoAsync(string maskedCardNo, CancellationToken ct = default);
    Task<CardLimitDefinition?> GetByBinAsync(string bin, CancellationToken ct = default);
    Task<List<CardLimitDefinition>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(CardLimitDefinition definition, CancellationToken ct = default);
    Task UpdateAsync(CardLimitDefinition definition, CancellationToken ct = default);
}