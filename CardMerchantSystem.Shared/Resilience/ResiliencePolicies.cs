using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace CardMerchantSystem.Shared.Resilience;

public static class ResiliencePolicies
{
    /// <summary>
    /// HTTP istekleri için retry policy
    /// </summary>
    public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempt
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                });
    }

    /// <summary>
    /// HTTP istekleri için circuit breaker policy
    /// </summary>
    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(
        int handledEventsAllowedBeforeBreaking = 5,
        int durationOfBreakInSeconds = 30)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(durationOfBreakInSeconds),
                onBreak: (outcome, timespan) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("Circuit breaker half-open");
                });
    }

    /// <summary>
    /// Genel retry policy (database, external services)
    /// </summary>
    public static AsyncRetryPolicy GetRetryPolicy(int retryCount = 3)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {exception.Message}");
                });
    }

    /// <summary>
    /// Genel circuit breaker policy
    /// </summary>
    public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy(
        int exceptionsAllowedBeforeBreaking = 5,
        int durationOfBreakInSeconds = 30)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(durationOfBreakInSeconds),
                onBreak: (exception, timespan) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {timespan.TotalSeconds}s due to {exception.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });
    }

    /// <summary>
    /// Timeout policy
    /// </summary>
    public static AsyncTimeoutPolicy GetTimeoutPolicy(int timeoutInSeconds = 30)
    {
        return Policy.TimeoutAsync(
            TimeSpan.FromSeconds(timeoutInSeconds),
            TimeoutStrategy.Pessimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                Console.WriteLine($"Timeout after {timespan.TotalSeconds}s");
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Kombinasyon: Retry + Circuit Breaker + Timeout
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedHttpPolicy(
        int retryCount = 3,
        int circuitBreakerThreshold = 5,
        int circuitBreakerDurationSeconds = 30,
        int timeoutSeconds = 30)
    {
        var retryPolicy = GetHttpRetryPolicy(retryCount);
        var circuitBreakerPolicy = GetHttpCircuitBreakerPolicy(circuitBreakerThreshold, circuitBreakerDurationSeconds);
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeoutSeconds));

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    }
}