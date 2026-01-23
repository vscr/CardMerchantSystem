// CardMerchantSystem.API/Middleware/CorrelationIdMiddleware.cs

namespace CardMerchantSystem.API.Middleware;

/// <summary>
/// Her request için unique correlation ID ekler
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Header'dan correlation ID al veya yeni oluştur
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        // Context'e ekle
        context.Items["CorrelationId"] = correlationId;

        // Response header'a ekle
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
            return Task.CompletedTask;
        });

        await _next(context);
    }
}