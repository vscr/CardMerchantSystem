using Transaction.Domain.Services;

namespace CardMerchantSystem.API.BackgroundServices;

/// <summary>
/// Günlük limit sıfırlama servisi - Her gün gece 00:01'de çalışır
/// </summary>
public class DailyLimitResetService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyLimitResetService> _logger;

    public DailyLimitResetService(
        IServiceProvider serviceProvider,
        ILogger<DailyLimitResetService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Günlük Limit Sıfırlama Servisi başlatıldı");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = GetNextRunTime(now);
            var delay = nextRun - now;

            _logger.LogInformation("Sonraki günlük limit sıfırlama zamanı: {NextRun}", nextRun);

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await ResetDailyLimitsAsync(stoppingToken);
            }
        }
    }

    private async Task ResetDailyLimitsAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Günlük limit sıfırlama başladı: {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var limitService = scope.ServiceProvider.GetRequiredService<ILimitService>();

            var result = await limitService.ResetDailyLimitsAsync(stoppingToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Günlük limitler başarıyla sıfırlandı");
            }
            else
            {
                _logger.LogWarning("Günlük limit sıfırlama başarısız: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Günlük limit sıfırlama sırasında hata oluştu");
        }
    }

    private static DateTime GetNextRunTime(DateTime now)
    {
        // Her gün 00:01'de çalış
        var tomorrow = now.Date.AddDays(1).AddMinutes(1);
        return tomorrow;
    }
}