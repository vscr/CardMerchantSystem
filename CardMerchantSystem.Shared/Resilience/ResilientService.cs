using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace CardMerchantSystem.Shared.Resilience;

public class ResilientService : IResilientService
{
    private readonly ILogger<ResilientService> _logger;
    private readonly ResilienceSettings _settings;
    private readonly AsyncPolicy _combinedPolicy;

    public ResilientService(ILogger<ResilientService> logger, IOptions<ResilienceSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        _combinedPolicy = BuildCombinedPolicy();
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string operationName)
    {
        var context = new Context(operationName);

        try
        {
            return await _combinedPolicy.ExecuteAsync(async (ctx) =>
            {
                _logger.LogDebug("Executing operation: {OperationName}", operationName);
                return await action();
            }, context);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open for operation: {OperationName}", operationName);
            throw;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout occurred for operation: {OperationName}", operationName);
            throw;
        }
    }

    public async Task ExecuteAsync(Func<Task> action, string operationName)
    {
        var context = new Context(operationName);

        try
        {
            await _combinedPolicy.ExecuteAsync(async (ctx) =>
            {
                _logger.LogDebug("Executing operation: {OperationName}", operationName);
                await action();
            }, context);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open for operation: {OperationName}", operationName);
            throw;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout occurred for operation: {OperationName}", operationName);
            throw;
        }
    }

    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int retryCount = 3)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(_settings.Retry.BaseDelaySeconds, retryAttempt)),
                onRetry: (exception, timespan, attempt, context) =>
                {
                    _logger.LogWarning(exception, "Retry attempt {Attempt} after {Delay}s", attempt, timespan.TotalSeconds);
                });

        return await retryPolicy.ExecuteAsync(action);
    }

    public async Task<T> ExecuteWithTimeoutAsync<T>(Func<CancellationToken, Task<T>> action, int timeoutSeconds = 30)
    {
        var timeoutPolicy = Policy.TimeoutAsync<T>(
            TimeSpan.FromSeconds(timeoutSeconds),
            TimeoutStrategy.Pessimistic);

        return await timeoutPolicy.ExecuteAsync(action, CancellationToken.None);
    }

    private AsyncPolicy BuildCombinedPolicy()
    {
        var retryPolicy = Policy
            .Handle<Exception>(ex => !(ex is BrokenCircuitException))
            .WaitAndRetryAsync(
                _settings.Retry.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(_settings.Retry.BaseDelaySeconds, retryAttempt)),
                onRetry: (exception, timespan, attempt, context) =>
                {
                    _logger.LogWarning(exception,
                        "Retry attempt {Attempt} for {OperationName} after {Delay}s",
                        attempt, context.OperationKey, timespan.TotalSeconds);
                });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                _settings.CircuitBreaker.EventsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(_settings.CircuitBreaker.DurationOfBreakSeconds),
                onBreak: (exception, timespan) =>
                {
                    _logger.LogError(exception, "Circuit breaker opened for {Duration}s", timespan.TotalSeconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker half-open, testing...");
                });

        var timeoutPolicy = Policy.TimeoutAsync(
            TimeSpan.FromSeconds(_settings.Timeout.DefaultTimeoutSeconds),
            TimeoutStrategy.Pessimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                _logger.LogWarning("Operation {OperationName} timed out after {Timeout}s",
                    context.OperationKey, timespan.TotalSeconds);
                return Task.CompletedTask;
            });

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    }
}