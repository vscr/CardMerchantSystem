using Transaction.Application.Commands;
using MediatR;

namespace CardMerchantSystem.API.Jobs;

/// <summary>
/// Günlük takas job'ı - Her gün 23:55'te çalışır
/// </summary>
public class SettlementJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<SettlementJob> _logger;

    public SettlementJob(IMediator mediator, ILogger<SettlementJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Günlük takas job'ı başladı: {Time}", DateTime.Now);

        try
        {
            var batchNumber = $"BATCH-{DateTime.Now:yyyyMMdd-HHmmss}";
            var command = new SettleTransactionsCommand(batchNumber);

            var result = await _mediator.Send(command);

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
            throw; // Hangfire retry için
        }
    }
}