namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;

public interface IFraudRuleRepository
{
    Task<FraudRule?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FraudRule?> GetByIdWithConditionsAsync(Guid id, CancellationToken ct = default);
    Task<FraudRule?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<FraudRule>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(FraudRule rule, CancellationToken ct = default);
    Task UpdateAsync(FraudRule rule, CancellationToken ct = default);
}