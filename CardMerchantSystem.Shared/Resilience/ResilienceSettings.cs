namespace CardMerchantSystem.Shared.Resilience;

public class ResilienceSettings
{
    public const string SectionName = "Resilience";

    public RetrySettings Retry { get; set; } = new();
    public CircuitBreakerSettings CircuitBreaker { get; set; } = new();
    public TimeoutSettings Timeout { get; set; } = new();
}

public class RetrySettings
{
    public int MaxRetryAttempts { get; set; } = 3;
    public int BaseDelaySeconds { get; set; } = 2;
}

public class CircuitBreakerSettings
{
    public int EventsAllowedBeforeBreaking { get; set; } = 5;
    public int DurationOfBreakSeconds { get; set; } = 30;
}

public class TimeoutSettings
{
    public int DefaultTimeoutSeconds { get; set; } = 30;
    public int LongRunningTimeoutSeconds { get; set; } = 120;
}