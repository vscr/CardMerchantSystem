using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace CardMerchantSystem.API.HealthChecks;

public static class HealthCheckConfiguration
{
    /// <summary>
    /// Health check servislerini register eder.
    /// 
    /// Tag'lar ile gruplama:
    /// - "critical": Uygulama çalışamaz (DB, Redis)
    /// - "supporting": Uygulama çalışır ama fonksiyon kaybı var (ELK, Hangfire)
    /// - "business": İş mantığı kontrolü (Fraud Engine)
    /// 
    /// /health          → tümü
    /// /health/ready    → sadece critical (Kubernetes readiness)
    /// /health/live     → basit alive check (Kubernetes liveness)
    /// </summary>
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;";

        var redisConnection = configuration.GetConnectionString("Redis")
            ?? "localhost:6379";

        services.AddHttpClient(); // Elasticsearch health check için

        services.AddHealthChecks()
          // ── Critical ──
          .AddSqlServer(
                  connectionString,
                  healthQuery: @"
                      SELECT 1 
                      WHERE EXISTS (SELECT 1 FROM fraud.FraudScenarios)
                        AND EXISTS (SELECT 1 FROM dbo.AuthUsers)
                        AND EXISTS (SELECT 1 FROM dbo.Transactions)",
                  name: "sqlserver",
                  tags: new[] { "critical", "database" },
                  timeout: TimeSpan.FromSeconds(5))
            .AddCheck<RedisHealthCheck>(
                "redis",
                tags: new[] { "critical", "cache" },
                timeout: TimeSpan.FromSeconds(5))

            // ── Supporting ──
            .AddCheck<ElasticsearchHealthCheck>(
                "elasticsearch",
                tags: new[] { "supporting", "logging" },
                timeout: TimeSpan.FromSeconds(5))
            .AddCheck<HangfireHealthCheck>(
                "hangfire",
                tags: new[] { "supporting", "jobs" },
                timeout: TimeSpan.FromSeconds(5))

            // ── Business ──
            .AddCheck<FraudEngineHealthCheck>(
                "fraud-engine",
                tags: new[] { "business", "fraud" },
                timeout: TimeSpan.FromSeconds(10));

        return services;
    }

    /// <summary>
    /// Health check endpoint'lerini map eder.
    /// 
    /// Endpoint'ler:
    /// GET /health       → Tüm kontroller (detaylı JSON)
    /// GET /health/ready → Sadece critical (DB, Redis) — Kubernetes readiness probe
    /// GET /health/live  → Basit alive check — Kubernetes liveness probe
    /// </summary>
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        // Tüm kontroller — detaylı JSON
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteDetailedResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Readiness — sadece critical servisler
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("critical"),
            ResponseWriter = WriteDetailedResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Liveness — basit alive check
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // Hiçbir check çalıştırma, sadece 200 dön
            ResponseWriter = (context, _) =>
            {
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(
                    JsonSerializer.Serialize(new
                    {
                        status = "Alive",
                        timestamp = DateTime.UtcNow
                    }));
            }
        });

        return app;
    }

    /// <summary>
    /// Detaylı JSON response writer.
    /// Her servisin durumu, süresi ve ek bilgileri döner.
    /// </summary>
    private static Task WriteDetailedResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = $"{report.TotalDuration.TotalMilliseconds:F1}ms",
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = $"{e.Value.Duration.TotalMilliseconds:F1}ms",
                description = e.Value.Description,
                tags = e.Value.Tags,
                error = e.Value.Exception?.Message,
                data = e.Value.Data?.Count > 0 ? e.Value.Data : null
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}