using CardMerchantSystem.Shared.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CardMerchantSystem.API.Services.External;

public class BkmApiSettings
{
    public const string SectionName = "ExternalServices:BKM";
    public string BaseUrl { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public int TimeoutSeconds { get; set; } = 30;
}

public interface IBkmApiClient
{
    Task<BkmAuthorizationResponse?> AuthorizeAsync(BkmAuthorizationRequest request, CancellationToken cancellationToken = default);
    Task<BkmSettlementResponse?> SettleAsync(BkmSettlementRequest request, CancellationToken cancellationToken = default);
}

public class BkmApiClient : IBkmApiClient
{
    private readonly IResilientHttpClient _httpClient;
    private readonly BkmApiSettings _settings;
    private readonly ILogger<BkmApiClient> _logger;

    public BkmApiClient(
        IResilientHttpClient httpClient,
        IOptions<BkmApiSettings> settings,
        ILogger<BkmApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<BkmAuthorizationResponse?> AuthorizeAsync(BkmAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("BKM Authorization request for card {CardMasked}", request.CardNumberMasked);

        var url = $"{_settings.BaseUrl}/api/authorize";
        return await _httpClient.PostAsync<BkmAuthorizationRequest, BkmAuthorizationResponse>(url, request, cancellationToken);
    }

    public async Task<BkmSettlementResponse?> SettleAsync(BkmSettlementRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("BKM Settlement request for batch {BatchNumber}", request.BatchNumber);

        var url = $"{_settings.BaseUrl}/api/settle";
        return await _httpClient.PostAsync<BkmSettlementRequest, BkmSettlementResponse>(url, request, cancellationToken);
    }
}

// DTOs
public record BkmAuthorizationRequest(
    string CardNumberMasked,
    decimal Amount,
    string Currency,
    string MerchantCode,
    string TerminalCode);

public record BkmAuthorizationResponse(
    bool IsApproved,
    string? AuthorizationCode,
    string? DeclineReason,
    string ResponseCode);

public record BkmSettlementRequest(
    string BatchNumber,
    DateTime SettlementDate,
    List<string> TransactionIds);

public record BkmSettlementResponse(
    bool IsSuccess,
    string? ErrorMessage,
    int ProcessedCount);