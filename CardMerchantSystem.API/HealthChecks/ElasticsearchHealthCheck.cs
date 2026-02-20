using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;

namespace CardMerchantSystem.API.HealthChecks;

/// <summary>
/// Elasticsearch sağlık kontrolü.
/// Loglama altyapısı — düşerse uygulama çalışır ama loglar kaybolur.
/// Bu yüzden Degraded (Healthy değil Unhealthy) döner.
/// </summary>
public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _elasticUrl;

    public ElasticsearchHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _elasticUrl = config["Serilog:WriteTo:4:Args:nodeUris"] ?? "http://localhost:9200";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var response = await client.GetAsync($"{_elasticUrl}/_cluster/health", ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                return HealthCheckResult.Degraded($"Elasticsearch HTTP {response.StatusCode}");

            // Cluster status: green/yellow/red
            if (body.Contains("\"status\":\"red\""))
                return HealthCheckResult.Degraded("Elasticsearch cluster RED");

            return HealthCheckResult.Healthy("Elasticsearch OK");
        }
        catch (Exception ex)
        {
            // Elasticsearch düşerse loglama etkilenir ama uygulama çalışır
            return HealthCheckResult.Degraded("Elasticsearch erişilemez", ex);
        }
    }
}