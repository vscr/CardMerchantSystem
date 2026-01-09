using Transaction.Domain.Services;

namespace CardMerchantSystem.API.Jobs;

/// <summary>
/// Aylık limit sıfırlama job'ı - Her ayın 1'inde 00:05'te çalışır
/// </summary>
public class MonthlyLimitResetJob
{
    private readonly ILimitService _limitService;
    private readonly ILogger<MonthlyLimitResetJob> _logger;

    public MonthlyLimitResetJob(ILimitService limitService, ILogger<MonthlyLimitResetJob> logger)
    {
        _limitService = limitService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Aylık limit sıfırlama job'ı başladı: {Time}", DateTime.Now);

        try
        {
            var result = await _limitService.ResetMonthlyLimitsAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Aylık limitler başarıyla sıfırlandı");
            }
            else
            {
                _logger.LogWarning("Aylık limit sıfırlama başarısız: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aylık limit sıfırlama sırasında hata oluştu");
            throw; // Hangfire retry için
        }
    }
}