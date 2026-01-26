using Polly;

namespace CardMerchantSystem.Shared.Resilience;

public interface IResilientService
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, string operationName);
    Task ExecuteAsync(Func<Task> action, string operationName);
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int retryCount = 3);
    Task<T> ExecuteWithTimeoutAsync<T>(Func<CancellationToken, Task<T>> action, int timeoutSeconds = 30);
}