using System.Text.Json.Serialization;

namespace CardMerchantSystem.API.Models;

/// <summary>
/// Standart API hata response modeli (RFC 7807 Problem Details benzeri)
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Hata kodu
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// Hata mesajı
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    /// <summary>
    /// HTTP status kodu
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Hata detayları (validasyon hataları için)
    /// </summary>
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// İstek ID'si (loglama için)
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    /// Hata zamanı
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// İstek path'i
    /// </summary>
    [JsonPropertyName("path")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Path { get; set; }

    public static ApiErrorResponse Create(string code, string message, int statusCode, string? traceId = null, string? path = null)
    {
        return new ApiErrorResponse
        {
            Code = code,
            Message = message,
            StatusCode = statusCode,
            TraceId = traceId,
            Path = path
        };
    }

    public static ApiErrorResponse CreateValidationError(IDictionary<string, string[]> errors, string? traceId = null, string? path = null)
    {
        return new ApiErrorResponse
        {
            Code = "VALIDATION_ERROR",
            Message = "Bir veya daha fazla validasyon hatası oluştu",
            StatusCode = 400,
            Errors = errors,
            TraceId = traceId,
            Path = path
        };
    }
}