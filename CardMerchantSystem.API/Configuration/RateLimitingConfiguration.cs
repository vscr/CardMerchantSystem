using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CardMerchantSystem.API.Configuration;

/// <summary>
/// Production-grade Rate Limiting Configuration
/// Türk bankacılık sektörü standartlarına uygun
/// </summary>
public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue<bool>("RateLimiting:Enabled", true);

        if (!isEnabled)
        {
            // Rate limiting tamamen kapalı (load test modu)
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetNoLimiter("disabled"));

                options.AddPolicy("Strict", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Standard", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Relaxed", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Auth", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Transaction", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Report", context => RateLimitPartition.GetNoLimiter("disabled"));
            });

            return services;
        }

        services.AddRateLimiter(options =>
        {
            // ══════════════════════════════════════════════════════════════
            // GLOBAL LIMITER - DDoS koruması (IP bazlı)
            // Gerçek sistemlerde bu genelde API Gateway'de yapılır
            // ══════════════════════════════════════════════════════════════
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(clientIp, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5000,              // IP başına 5000 token
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    TokensPerPeriod = 5000,         // Saniyede 5000 token yenilenir
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 100
                });
            });

            // ══════════════════════════════════════════════════════════════
            // AUTH POLICY - Brute force koruması
            // Login/Register için SIKI limit (gerçekçi)
            // ══════════════════════════════════════════════════════════════
            options.AddPolicy("Auth", context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(clientIp, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 10,               // 10 deneme
                    Window = TimeSpan.FromMinutes(5), // 5 dakikada
                    SegmentsPerWindow = 5,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0                  // Kuyruk yok, direkt reddet
                });
            });

            // Backward compatibility için "Strict" = "Auth"
            options.AddPolicy("Strict", context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetSlidingWindowLimiter(clientIp, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(5),
                    SegmentsPerWindow = 5,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            // ══════════════════════════════════════════════════════════════
            // TRANSACTION POLICY - Yüksek throughput
            // Gerçek bankacılık: Transaction'a LIMIT KONMAZ
            // Ama merchant bazlı soft limit olabilir
            // ══════════════════════════════════════════════════════════════
            options.AddPolicy("Transaction", context =>
            {
                // Merchant bazlı limit (header veya claim'den al)
                var merchantId = context.Request.Headers["X-Merchant-Id"].FirstOrDefault()
                    ?? context.User?.FindFirst("merchant_id")?.Value
                    ?? "default";

                return RateLimitPartition.GetTokenBucketLimiter(merchantId, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 1000,              // Merchant başına 1000 TPS
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    TokensPerPeriod = 1000,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 50                 // 50 istek kuyruğa alınabilir
                });
            });

            // ══════════════════════════════════════════════════════════════
            // STANDARD POLICY - Normal CRUD işlemleri
            // Merchant, Terminal, Card yönetimi vs.
            // ══════════════════════════════════════════════════════════════
            options.AddPolicy("Standard", context =>
            {
                var username = context.User?.Identity?.Name ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 300,              // Kullanıcı başına 300/dakika
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10
                });
            });

            // ══════════════════════════════════════════════════════════════
            // RELAXED POLICY - Read-heavy endpoint'ler
            // Liste, arama, dashboard vs.
            // ══════════════════════════════════════════════════════════════
            options.AddPolicy("Relaxed", context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1000,             // IP başına 1000/dakika
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 20
                });
            });

            // ══════════════════════════════════════════════════════════════
            // REPORT POLICY - Ağır raporlar
            // CPU/DB yoğun sorgular için düşük limit
            // ══════════════════════════════════════════════════════════════
            options.AddPolicy("Report", context =>
            {
                var username = context.User?.Identity?.Name ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20,               // Kullanıcı başına 20/dakika
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                });
            });

            // ══════════════════════════════════════════════════════════════
            // REJECTED RESPONSE
            // ══════════════════════════════════════════════════════════════
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry)
                    ? (int)retry.TotalSeconds
                    : 60;

                context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

                var response = new
                {
                    code = "RATE_LIMIT_EXCEEDED",
                    message = "İstek limiti aşıldı. Lütfen bekleyin.",
                    retryAfterSeconds = retryAfter,
                    timestamp = DateTime.UtcNow
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
        });

        return services;
    }
}