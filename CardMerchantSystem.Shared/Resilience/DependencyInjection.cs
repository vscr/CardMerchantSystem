using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CardMerchantSystem.Shared.Resilience;

public static class DependencyInjection
{
    public static IServiceCollection AddResilienceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings
        services.Configure<ResilienceSettings>(configuration.GetSection(ResilienceSettings.SectionName));

        // Core Services
        services.AddSingleton<IResilientService, ResilientService>();

        // Resilient HTTP Client
        services.AddHttpClient<IResilientHttpClient, ResilientHttpClient>()
            .AddResilienceHandler();

        return services;
    }

    /// <summary>
    /// HttpClient'a Polly policy ekler
    /// </summary>
    public static IHttpClientBuilder AddResilienceHandler(this IHttpClientBuilder builder,
        int retryCount = 3,
        int circuitBreakerThreshold = 5,
        int circuitBreakerDurationSeconds = 30,
        int timeoutSeconds = 30)
    {
        return builder.AddPolicyHandler(
            ResiliencePolicies.GetCombinedHttpPolicy(
                retryCount,
                circuitBreakerThreshold,
                circuitBreakerDurationSeconds,
                timeoutSeconds));
    }

    /// <summary>
    /// Named HttpClient için Polly policy ekler
    /// </summary>
    public static IServiceCollection AddResilientHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        string name,
        Action<HttpClient> configureClient,
        int retryCount = 3,
        int circuitBreakerThreshold = 5,
        int circuitBreakerDurationSeconds = 30,
        int timeoutSeconds = 30)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>(name, configureClient)
            .AddResilienceHandler(retryCount, circuitBreakerThreshold, circuitBreakerDurationSeconds, timeoutSeconds);

        return services;
    }
}