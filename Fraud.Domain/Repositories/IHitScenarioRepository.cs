namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;

public interface IHitScenarioRepository
{
    Task<List<HitScenario>> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct = default);
    Task<List<HitScenario>> GetByCardNoAsync(string maskedCardNo, int days = 30, CancellationToken ct = default);
    Task<int> GetCountByScenarioIdAsync(Guid scenarioId, DateTime from, CancellationToken ct = default);
    Task AddAsync(HitScenario hitScenario, CancellationToken ct = default);
    Task AddRangeAsync(List<HitScenario> hitScenarios, CancellationToken ct = default);
}