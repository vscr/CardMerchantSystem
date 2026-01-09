using Transaction.Domain.Services;

namespace CardMerchantSystem.API.Jobs;

/// <summary>
/// Günlük limit sıfırlama job'ı - Her gün 00:01'de çalışır
/// </summary>
public class DailyLimitResetJob
{
    private readonly ILimitService _limitService;
    private readonly ILogger<DailyLimitResetJob> _logger;

    public DailyLimitResetJob(ILimitService limitService, ILogger<DailyLimitResetJob> logger)
    {
        _limitService = limitService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Günlük limit sıfırlama job'ı başladı: {Time}", DateTime.Now);

        try
        {
            var result = await _limitService.ResetDailyLimitsAsync();

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
            throw; // Hangfire retry için
        }
    }
}