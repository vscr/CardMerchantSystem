using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CardMerchantSystem.Shared.Resilience;

public interface IResilientHttpClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}

public class ResilientHttpClient : IResilientHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IResilientService _resilientService;
    private readonly ILogger<ResilientHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ResilientHttpClient(
        HttpClient httpClient,
        IResilientService resilientService,
        ILogger<ResilientHttpClient> logger)
    {
        _httpClient = httpClient;
        _resilientService = resilientService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        return await _resilientService.ExecuteAsync(async () =>
        {
            _logger.LogDebug("GET request to {Url}", url);
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
        }, $"GET:{url}");
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        return await _resilientService.ExecuteAsync(async () =>
        {
            _logger.LogDebug("POST request to {Url}", url);
            var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
        }, $"POST:{url}");
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return await _resilientService.ExecuteAsync(async () =>
        {
            _logger.LogDebug("{Method} request to {Url}", request.Method, request.RequestUri);
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }, $"{request.Method}:{request.RequestUri}");
    }
}