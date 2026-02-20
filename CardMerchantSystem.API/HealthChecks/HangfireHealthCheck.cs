using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CardMerchantSystem.API.HealthChecks;

public class HangfireHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var monitor = JobStorage.Current.GetMonitoringApi();
            var servers = monitor.Servers();

            if (!servers.Any())
                return Task.FromResult(HealthCheckResult.Degraded("Hangfire server çalışmıyor"));

            var activeServer = servers.FirstOrDefault(s =>
                s.Heartbeat > DateTime.UtcNow.AddMinutes(-2));

            if (activeServer == null)
                return Task.FromResult(HealthCheckResult.Degraded("Hangfire server heartbeat yok (>2dk)"));

            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Hangfire OK — {servers.Count} server, {recurringJobs.Count} recurring jobs"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Hangfire hatası", ex));
        }
    }
}