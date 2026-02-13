namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;

public interface IFraudBlacklistRepository
{
    Task<FraudBlacklist?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<FraudBlacklist>> GetByListTypeAsync(string listType, CancellationToken ct = default);
    Task<bool> ExistsAsync(string listType, string value, CancellationToken ct = default);
    Task<List<FraudBlacklist>> GetActiveBlacklistAsync(string listType, CancellationToken ct = default);
    Task<List<FraudBlacklist>> GetActiveWhitelistAsync(string listType, CancellationToken ct = default);
    Task AddAsync(FraudBlacklist entry, CancellationToken ct = default);
    Task UpdateAsync(FraudBlacklist entry, CancellationToken ct = default);
}