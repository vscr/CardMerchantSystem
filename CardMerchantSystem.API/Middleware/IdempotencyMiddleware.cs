using CardMerchantSystem.Shared.Idempotency;

namespace CardMerchantSystem.API.Middleware;

/// <summary>
/// HTTP header'dan Idempotency-Key'i okuyup context'e enjekte eder.
/// 
/// Kullanım (POS/Client tarafı):
/// ──────────────────────────────
/// POST /api/Transactions/process
/// Idempotency-Key: T1017506:260214123456
/// Content-Type: application/json
/// 
/// İlk istek → 200 OK { ... }
/// Aynı key tekrar → 200 OK { aynı sonuç, cache'den }
/// Aynı key + farklı body → 409 Conflict
/// Key olmadan → normal akış (backward compatible)
/// 
/// Response header'da bilgi döner:
/// X-Idempotency-Status: Hit|Miss|New
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IIdempotencyContext idempotencyContext)
    {
        // Sadece POST/PUT isteklerinde çalış
        if (context.Request.Method is "POST" or "PUT")
        {
            var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (!string.IsNullOrEmpty(key))
            {
                // Key validation (max 200 karakter, güvenli karakterler)
                if (key.Length > 200)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Idempotency-Key çok uzun (max 200 karakter)"
                    });
                    return;
                }

                idempotencyContext.IdempotencyKey = key;
            }
        }

        await _next(context);
    }
}