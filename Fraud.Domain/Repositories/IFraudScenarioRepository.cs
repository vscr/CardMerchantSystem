namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;
using Fraud.Domain.Enums;

public interface IFraudScenarioRepository
{
    Task<FraudScenario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<FraudScenario>> GetAllAsync(CancellationToken ct = default);
    Task<List<FraudScenario>> GetActiveByModeAsync(FraudCheckMode mode, CancellationToken ct = default);
    Task<int> GetNextScenarioNoAsync(CancellationToken ct = default);
    Task AddAsync(FraudScenario scenario, CancellationToken ct = default);
    Task UpdateAsync(FraudScenario scenario, CancellationToken ct = default);
}