using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CardMerchantSystem.API.Configuration;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Check if rate limiting is enabled
        var isEnabled = configuration.GetValue<bool>("RateLimiting:Enabled", true);

        if (!isEnabled)
        {
            // Rate limiting disabled - add dummy/no-op limiter
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetNoLimiter("disabled"));

                // Add no-op policies so [EnableRateLimiting] attributes don't fail
                options.AddPolicy("Strict", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Standard", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Relaxed", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("PerUser", context => RateLimitPartition.GetNoLimiter("disabled"));
                options.AddPolicy("Transaction", context => RateLimitPartition.GetNoLimiter("disabled"));
            });

            return services;
        }

        services.AddRateLimiter(options =>
        {
            // Global limiter - Tüm API için
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                // IP bazlı partition
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = configuration.GetValue<int>("RateLimiting:Global:PermitLimit", 100),
                    Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimiting:Global:WindowMinutes", 1)),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10
                });
            });

            // Policy: Strict - Login, Register gibi hassas endpoint'ler
            options.AddFixedWindowLimiter("Strict", opt =>
            {
                opt.PermitLimit = configuration.GetValue<int>("RateLimiting:Strict:PermitLimit", 5);
                opt.Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimiting:Strict:WindowMinutes", 1));
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // Policy: Standard - Normal CRUD işlemleri
            options.AddFixedWindowLimiter("Standard", opt =>
            {
                opt.PermitLimit = configuration.GetValue<int>("RateLimiting:Standard:PermitLimit", 60);
                opt.Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimiting:Standard:WindowMinutes", 1));
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 5;
            });

            // Policy: Relaxed - Okuma ağırlıklı endpoint'ler
            options.AddFixedWindowLimiter("Relaxed", opt =>
            {
                opt.PermitLimit = 200;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 10;
            });

            // Policy: PerUser - Kullanıcı bazlı limit
            options.AddPolicy("PerUser", context =>
            {
                var username = context.User?.Identity?.Name ?? "anonymous";

                return RateLimitPartition.GetTokenBucketLimiter(username, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    TokensPerPeriod = 100,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10
                });
            });

            // Policy: Transaction - İşlem endpoint'leri için özel
            options.AddSlidingWindowLimiter("Transaction", opt =>
            {
                opt.PermitLimit = 30;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow = 6; // 10 saniyelik segmentler
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 5;
            });

            // Rejected response
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = new
                {
                    code = "RATE_LIMIT_EXCEEDED",
                    message = "Çok fazla istek gönderdiniz. Lütfen bekleyin.",
                    retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                        ? retryAfter.TotalSeconds
                        : 60
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
        });

        return services;
    }
}