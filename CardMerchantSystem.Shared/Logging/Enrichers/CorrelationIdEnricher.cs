// CardMerchantSystem.Shared/Logging/Enrichers/CorrelationIdEnricher.cs

using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace CardMerchantSystem.Shared.Logging.Enrichers;

/// <summary>
/// Correlation ID enricher
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        // Correlation ID
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", correlationId));
        }

        // User ID
        var userId = httpContext.User?.FindFirst("sub")?.Value
                     ?? httpContext.User?.FindFirst("UserId")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
        }

        // Username
        var username = httpContext.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(username))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", username));
        }

        // IP Address
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("IpAddress", ipAddress));
        }

        // Request Path
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path));

        // HTTP Method
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("HttpMethod", httpContext.Request.Method));
    }
}