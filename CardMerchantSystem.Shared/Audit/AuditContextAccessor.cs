using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// HttpContext'ten audit bilgilerini alan accessor.
/// </summary>
public class AuditContextAccessor : IAuditContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string PendingAuditEntriesKey = "PendingAuditEntries";

    public AuditContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

    public string? UserName => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.Name)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value
        ?? _httpContextAccessor.HttpContext?.User.Identity?.Name;

    public string? IpAddress => GetClientIpAddress();

    public string? CorrelationId => _httpContextAccessor.HttpContext?
        .Items.TryGetValue("CorrelationId", out var correlationId) == true
            ? correlationId?.ToString()
            : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault();

    public void SetPendingAuditEntries(List<AuditEntry> entries)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Items[PendingAuditEntriesKey] = entries;
        }
    }

    public List<AuditEntry>? GetPendingAuditEntries()
    {
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue(PendingAuditEntriesKey, out var entries) == true)
        {
            return entries as List<AuditEntry>;
        }
        return null;
    }

    public void ClearPendingAuditEntries()
    {
        _httpContextAccessor.HttpContext?.Items.Remove(PendingAuditEntriesKey);
    }

    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // X-Forwarded-For header kontrolü (proxy/load balancer arkasında)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // X-Real-IP header kontrolü
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Direkt connection IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}