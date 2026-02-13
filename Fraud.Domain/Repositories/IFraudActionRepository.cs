namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;

public interface IFraudActionRepository
{
    Task<List<FraudAction>> GetByAlertIdAsync(Guid alertId, CancellationToken ct = default);
    Task<List<FraudAction>> GetByCardNoAsync(string maskedCardNo, int days = 90, CancellationToken ct = default);
    Task AddAsync(FraudAction action, CancellationToken ct = default);
}