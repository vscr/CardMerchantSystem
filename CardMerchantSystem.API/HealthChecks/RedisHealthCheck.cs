using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace CardMerchantSystem.API.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisHealthCheck(IConnectionMultiplexer redis) => _redis = redis;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();

            // 1. PING
            var latency = await db.PingAsync();

            // 2. SET/GET test
            var testKey = "health:check";
            await db.StringSetAsync(testKey, DateTime.UtcNow.Ticks.ToString(), TimeSpan.FromSeconds(10));
            var value = await db.StringGetAsync(testKey);

            if (value.IsNullOrEmpty)
                return HealthCheckResult.Degraded("Redis SET/GET başarısız");

            if (latency.TotalMilliseconds > 100)
                return HealthCheckResult.Degraded($"Redis yavaş: {latency.TotalMilliseconds:F1}ms");

            return HealthCheckResult.Healthy($"Redis OK — {latency.TotalMilliseconds:F1}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis bağlantı hatası", ex);
        }
    }
}