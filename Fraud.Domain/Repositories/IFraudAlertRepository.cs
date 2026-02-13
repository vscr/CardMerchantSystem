namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;
using Fraud.Domain.Enums;

public interface IFraudAlertRepository
{
    Task<FraudAlert?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FraudAlert?> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct = default);
    Task<List<FraudAlert>> GetByStatusAsync(FraudAlertStatus status, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<List<FraudAlert>> GetAssignedToAsync(string operatorUsername, CancellationToken ct = default);
    Task<int> GetCountByStatusAsync(FraudAlertStatus status, CancellationToken ct = default);
    Task AddAsync(FraudAlert alert, CancellationToken ct = default);
    Task UpdateAsync(FraudAlert alert, CancellationToken ct = default);
}