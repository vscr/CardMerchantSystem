using Fraud.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Fraud.Application.Services;

public class BlacklistService : IBlacklistService
{
    private readonly IFraudBlacklistRepository _repo;
    private readonly ILogger<BlacklistService> _logger;

    public BlacklistService(IFraudBlacklistRepository repo, ILogger<BlacklistService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<bool> IsBlacklistedAsync(string listType, string value, CancellationToken ct = default)
    {
        var entries = await _repo.GetActiveBlacklistAsync(listType, ct);
        var isBlacklisted = entries.Any(e => e.IsEffective(DateTime.UtcNow) &&
            string.Equals(e.Value, value, StringComparison.OrdinalIgnoreCase));

        if (isBlacklisted)
            _logger.LogWarning("Kara listede bulundu: Type={Type}, Value={Value}", listType, value);

        return isBlacklisted;
    }

    public async Task<bool> IsWhitelistedAsync(string listType, string value, CancellationToken ct = default)
    {
        var entries = await _repo.GetActiveWhitelistAsync(listType, ct);
        return entries.Any(e => e.IsEffective(DateTime.UtcNow) &&
            string.Equals(e.Value, value, StringComparison.OrdinalIgnoreCase));
    }
}