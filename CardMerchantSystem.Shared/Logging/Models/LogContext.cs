// CardMerchantSystem.Shared/Logging/Models/LogContext.cs

namespace CardMerchantSystem.Shared.Logging.Models;

/// <summary>
/// Log context bilgileri
/// </summary>
public class LogContext
{
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
}