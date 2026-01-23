// CardMerchantSystem.Shared/Logging/PerformanceLogger.cs

using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CardMerchantSystem.Shared.Logging;

/// <summary>
/// Performance logging için helper
/// </summary>
public class PerformanceLogger : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private readonly Dictionary<string, object> _properties;

    public PerformanceLogger(ILogger logger, string operationName, Dictionary<string, object>? properties = null)
    {
        _logger = logger;
        _operationName = operationName;
        _properties = properties ?? new Dictionary<string, object>();
        _stopwatch = Stopwatch.StartNew();
    }

    public void AddProperty(string key, object value)
    {
        _properties[key] = value;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        var elapsedMs = _stopwatch.ElapsedMilliseconds;

        var logLevel = elapsedMs switch
        {
            > 5000 => LogLevel.Warning, // 5 saniyeden uzun
            > 2000 => LogLevel.Information, // 2-5 saniye arası
            _ => LogLevel.Debug // 2 saniyeden kısa
        };

        _properties["ElapsedMilliseconds"] = elapsedMs;
        _properties["OperationName"] = _operationName;

        _logger.Log(logLevel,
            "Operation {OperationName} completed in {ElapsedMilliseconds}ms. Properties: {@Properties}",
            _operationName,
            elapsedMs,
            _properties);
    }
}

/// <summary>
/// Extension methods for PerformanceLogger
/// </summary>
public static class PerformanceLoggerExtensions
{
    public static PerformanceLogger TrackPerformance(
        this ILogger logger,
        string operationName,
        Dictionary<string, object>? properties = null)
    {
        return new PerformanceLogger(logger, operationName, properties);
    }
}