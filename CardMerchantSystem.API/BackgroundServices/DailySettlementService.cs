using Transaction.Application.Commands;
using MediatR;

namespace CardMerchantSystem.API.BackgroundServices;

/// <summary>
/// Günlük takas servisi - Her gün gece 23:55'te çalışır
/// </summary>
public class DailySettlementService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailySettlementService> _logger;

    public DailySettlementService(
        IServiceProvider serviceProvider,
        ILogger<DailySettlementService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Günlük Takas Servisi başlatıldı");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = GetNextRunTime(now);
            var delay = nextRun - now;

            _logger.LogInformation("Sonraki takas zamanı: {NextRun}", nextRun);

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await RunSettlementAsync(stoppingToken);
            }
        }
    }

    private async Task RunSettlementAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Günlük takas işlemi başladı: {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var batchNumber = $"BATCH-{DateTime.Now:yyyyMMdd-HHmmss}";
            var command = new SettleTransactionsCommand(batchNumber);

            var result = await mediator.Send(command, stoppingToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Günlük takas tamamlandı. Batch: {BatchNumber}, Toplam: {Total}, Başarılı: {Success}, Tutar: {Amount:N2} TRY",
                    result.Value!.BatchNumber,
                    result.Value.TotalTransactions,
                    result.Value.SuccessfulSettlements,
                    result.Value.TotalAmount);
            }
            else
            {
                _logger.LogWarning("Günlük takas başarısız: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Günlük takas sırasında hata oluştu");
        }
    }

    private static DateTime GetNextRunTime(DateTime now)
    {
        // Her gün 23:55'te çalış
        var today = now.Date.AddHours(23).AddMinutes(55);

        if (now >= today)
        {
            // Bugünkü zaman geçtiyse yarın çalış
            return today.AddDays(1);
        }

        return today;
    }
}