namespace Fraud.Application.Services;

/// <summary>
/// Kara/beyaz liste kontrol servisi.
/// Rule engine içinden çağrılır.
/// </summary>
public interface IBlacklistService
{
    Task<bool> IsBlacklistedAsync(string listType, string value, CancellationToken ct = default);
    Task<bool> IsWhitelistedAsync(string listType, string value, CancellationToken ct = default);
}