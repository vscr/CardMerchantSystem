using CardMerchantSystem.Shared.Kernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Transaction.Application.DTOs;
using Transaction.Infrastructure.Dapper;

namespace CardMerchantSystem.API.Controllers.Transaction;

/// <summary>
/// Dapper tabanlı yüksek performanslı Transaction endpoint'leri
/// EF Core yerine raw SQL kullanarak 5-15x daha hızlı sonuç döner
/// </summary>
[Route("api/transactions/fast")]
[Authorize]
public class TransactionsFastController : ApiControllerBase
{
    private readonly ITransactionReadRepository _readRepository;
    private readonly ITransactionReportRepository _reportRepository;
    private readonly ILogger<TransactionsFastController> _logger;

    public TransactionsFastController(
        ITransactionReadRepository readRepository,
        ITransactionReportRepository reportRepository,
        ILogger<TransactionsFastController> logger)
    {
        _readRepository = readRepository;
        _reportRepository = reportRepository;
        _logger = logger;
    }

    /// <summary>
    /// [DAPPER] Health check - Load test sırasında read çalışıyor mu?
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> HealthCheck(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var filter = new TransactionQueryFilter { PageNumber = 1, PageSize = 1 };
            var result = await _readRepository.GetPagedAsync(filter, cancellationToken);

            sw.Stop();

            return Ok(new
            {
                status = "healthy",
                dapperWorking = true,
                totalTransactions = result.TotalCount,
                queryTimeMs = sw.ElapsedMilliseconds,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Health check failed");

            return Ok(new
            {
                status = "unhealthy",
                dapperWorking = false,
                error = ex.Message,
                queryTimeMs = sw.ElapsedMilliseconds,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// [DAPPER] Sayfalı işlem listesi - Yüksek performans
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TransactionDto>), 200)]
    public async Task<ActionResult<PagedResponse<TransactionDto>>> GetPagedFast(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? merchantId = null,
        [FromQuery] Guid? terminalId = null,
        [FromQuery] string? cardNumberMasked = null,
        [FromQuery] int? statusId = null,
        [FromQuery] int? transactionTypeId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var filter = new TransactionQueryFilter
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            MerchantId = merchantId,
            TerminalId = terminalId,
            CardNumberMasked = cardNumberMasked,
            StatusId = statusId,
            TransactionTypeId = transactionTypeId,
            StartDate = startDate,
            EndDate = endDate,
            MinAmount = minAmount,
            MaxAmount = maxAmount
        };

        var result = await _readRepository.GetPagedAsync(filter, cancellationToken);
        var dtos = result.Items.Select(MapToDto).ToList();
        var response = PagedResponse<TransactionDto>.Create(dtos, result.TotalCount, result.PageNumber, result.PageSize);

        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());
        _logger.LogInformation("[DAPPER] GetPaged completed in {ElapsedMs}ms - {Count} items", sw.ElapsedMilliseconds, dtos.Count);

        return Ok(response);
    }

    /// <summary>
    /// [DAPPER] ID ile işlem getir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TransactionDto>> GetByIdFast(Guid id, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _readRepository.GetByIdAsync(id, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        if (result == null)
            return NotFound(new ApiErrorResponse("İşlem bulunamadı", "NOT_FOUND"));

        return Ok(MapToDto(result));
    }

    /// <summary>
    /// [DAPPER] Referans numarası ile işlem getir
    /// </summary>
    [HttpGet("by-reference/{referenceNumber}")]
    [ProducesResponseType(typeof(TransactionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TransactionDto>> GetByReferenceNumberFast(string referenceNumber, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _readRepository.GetByReferenceNumberAsync(referenceNumber, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        if (result == null)
            return NotFound(new ApiErrorResponse("İşlem bulunamadı", "NOT_FOUND"));

        return Ok(MapToDto(result));
    }

    /// <summary>
    /// [DAPPER] Merchant'a ait son işlemler
    /// </summary>
    [HttpGet("by-merchant/{merchantId:guid}/recent")]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), 200)]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetRecentByMerchant(Guid merchantId, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _readRepository.GetRecentByMerchantAsync(merchantId, limit, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(MapToDto));
    }

    /// <summary>
    /// [DAPPER] Takas bekleyen işlemler
    /// </summary>
    [HttpGet("pending-settlement")]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), 200)]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetPendingSettlement(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _readRepository.GetPendingSettlementAsync(cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());
        _logger.LogInformation("[DAPPER] GetPendingSettlement completed in {ElapsedMs}ms - {Count} items", sw.ElapsedMilliseconds, result.Count());

        return Ok(result.Select(MapToDto));
    }

    /// <summary>
    /// [DAPPER] Kart günlük işlem toplamı (Limit kontrolü için)
    /// </summary>
    [HttpGet("card-daily-total/{cardNumberMasked}")]
    [ProducesResponseType(typeof(CardLimitTotalDto), 200)]
    public async Task<ActionResult<CardLimitTotalDto>> GetCardDailyTotal(string cardNumberMasked, [FromQuery] DateTime? date = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var targetDate = date ?? DateTime.UtcNow.Date;
        var total = await _readRepository.GetDailyTotalByCardAsync(cardNumberMasked, targetDate, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(new CardLimitTotalDto
        {
            CardNumberMasked = cardNumberMasked,
            PeriodType = "Daily",
            PeriodStart = targetDate,
            PeriodEnd = targetDate.AddDays(1).AddTicks(-1),
            TotalAmount = total,
            QueryTimeMs = sw.ElapsedMilliseconds
        });
    }

    /// <summary>
    /// [DAPPER] Kart aylık işlem toplamı
    /// </summary>
    [HttpGet("card-monthly-total/{cardNumberMasked}")]
    [ProducesResponseType(typeof(CardLimitTotalDto), 200)]
    public async Task<ActionResult<CardLimitTotalDto>> GetCardMonthlyTotal(string cardNumberMasked, [FromQuery] int? year = null, [FromQuery] int? month = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var now = DateTime.UtcNow;
        var targetYear = year ?? now.Year;
        var targetMonth = month ?? now.Month;
        var total = await _readRepository.GetMonthlyTotalByCardAsync(cardNumberMasked, targetYear, targetMonth, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        var periodStart = new DateTime(targetYear, targetMonth, 1);
        return Ok(new CardLimitTotalDto
        {
            CardNumberMasked = cardNumberMasked,
            PeriodType = "Monthly",
            PeriodStart = periodStart,
            PeriodEnd = periodStart.AddMonths(1).AddTicks(-1),
            TotalAmount = total,
            QueryTimeMs = sw.ElapsedMilliseconds
        });
    }

    /// <summary>
    /// [DAPPER] İşlem istatistikleri (Dashboard için)
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(TransactionStatsDto), 200)]
    public async Task<ActionResult<TransactionStatsDto>> GetStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid? merchantId = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var stats = await _reportRepository.GetStatsAsync(startDate, endDate, merchantId, cancellationToken);
        var byType = await _reportRepository.GetStatsByTransactionTypeAsync(startDate, endDate, merchantId, cancellationToken);
        var dailyTrend = await _reportRepository.GetDailyTrendAsync(startDate, endDate, merchantId, cancellationToken);

        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());
        _logger.LogInformation("[DAPPER] GetStats completed in {ElapsedMs}ms - Total: {Total}", sw.ElapsedMilliseconds, stats.TotalCount);

        var response = new TransactionStatsDto
        {
            TotalCount = stats.TotalCount,
            TotalAmount = stats.TotalAmount,
            SuccessfulCount = stats.SuccessfulCount,
            SuccessfulAmount = stats.SuccessfulAmount,
            DeclinedCount = stats.DeclinedCount,
            DeclinedAmount = stats.DeclinedAmount,
            RefundedCount = stats.RefundedCount,
            RefundedAmount = stats.RefundedAmount,
            ReversedCount = stats.ReversedCount,
            ReversedAmount = stats.ReversedAmount,
            PendingCount = stats.PendingCount,
            SettledCount = stats.SettledCount,
            SettledAmount = stats.SettledAmount,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            GeneratedAt = DateTime.UtcNow,
            ByTransactionType = byType.Select(t => new TransactionTypeStatsDto
            {
                TransactionType = t.TransactionType,
                TransactionTypeDisplayName = t.TransactionTypeDisplayName,
                Count = t.Count,
                Amount = t.Amount,
                Percentage = stats.TotalCount > 0 ? Math.Round((decimal)t.Count / stats.TotalCount * 100, 2) : 0
            }).ToList(),
            DailyTrend = dailyTrend.Select(d => new DailyTransactionStatsDto
            {
                Date = d.Date,
                Count = d.Count,
                Amount = d.Amount,
                SuccessfulCount = d.SuccessfulCount,
                DeclinedCount = d.DeclinedCount
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// [DAPPER] Günlük işlem trendi
    /// </summary>
    [HttpGet("stats/daily-trend")]
    [ProducesResponseType(typeof(IEnumerable<DailyTransactionStatsDto>), 200)]
    public async Task<ActionResult<IEnumerable<DailyTransactionStatsDto>>> GetDailyTrend([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid? merchantId = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetDailyTrendAsync(startDate, endDate, merchantId, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(d => new DailyTransactionStatsDto { Date = d.Date, Count = d.Count, Amount = d.Amount, SuccessfulCount = d.SuccessfulCount, DeclinedCount = d.DeclinedCount }));
    }

    /// <summary>
    /// [DAPPER] Saatlik işlem dağılımı
    /// </summary>
    [HttpGet("stats/hourly-distribution")]
    [ProducesResponseType(typeof(IEnumerable<HourlyDistributionDto>), 200)]
    public async Task<ActionResult<IEnumerable<HourlyDistributionDto>>> GetHourlyDistribution([FromQuery] DateTime? date = null, [FromQuery] Guid? merchantId = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetHourlyDistributionAsync(date ?? DateTime.UtcNow.Date, merchantId, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(h => new HourlyDistributionDto { Hour = h.Hour, Count = h.Count, Amount = h.Amount }));
    }

    /// <summary>
    /// [DAPPER] Top merchant'lar
    /// </summary>
    [HttpGet("stats/top-merchants")]
    [ProducesResponseType(typeof(IEnumerable<TopMerchantDto>), 200)]
    public async Task<ActionResult<IEnumerable<TopMerchantDto>>> GetTopMerchants([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetTopMerchantsAsync(startDate, endDate, limit, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(m => new TopMerchantDto { MerchantId = m.MerchantId, MerchantCode = m.MerchantCode, TransactionCount = m.TransactionCount, TotalAmount = m.TotalAmount, SuccessfulAmount = m.SuccessfulAmount, SuccessRate = m.SuccessRate }));
    }

    /// <summary>
    /// [DAPPER] Red nedeni istatistikleri
    /// </summary>
    [HttpGet("stats/decline-reasons")]
    [ProducesResponseType(typeof(IEnumerable<DeclineReasonStatsDto>), 200)]
    public async Task<ActionResult<IEnumerable<DeclineReasonStatsDto>>> GetDeclineReasonStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid? merchantId = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetDeclineReasonStatsAsync(startDate, endDate, merchantId, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(d => new DeclineReasonStatsDto { DeclineReason = d.DeclineReason, DeclineReasonDisplayName = d.DeclineReasonDisplayName, Count = d.Count, Amount = d.Amount }));
    }

    /// <summary>
    /// [DAPPER] Settlement batch özeti
    /// </summary>
    [HttpGet("stats/settlement-batches")]
    [ProducesResponseType(typeof(IEnumerable<TransactionSettlementBatchDto>), 200)]
    public async Task<ActionResult<IEnumerable<TransactionSettlementBatchDto>>> GetSettlementBatches([FromQuery] string? batchNumber = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetSettlementBatchSummaryAsync(batchNumber, startDate, endDate, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(b => new TransactionSettlementBatchDto { BatchNumber = b.BatchNumber, TransactionCount = b.TransactionCount, TotalAmount = b.TotalAmount, FirstTransaction = b.FirstTransaction, LastTransaction = b.LastTransaction, SettledAt = b.SettledAt }));
    }

    /// <summary>
    /// [DAPPER] Fraud istatistikleri
    /// </summary>
    [HttpGet("stats/fraud")]
    [ProducesResponseType(typeof(IEnumerable<FraudStatsDto>), 200)]
    public async Task<ActionResult<IEnumerable<FraudStatsDto>>> GetFraudStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid? merchantId = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var result = await _reportRepository.GetFraudStatsAsync(startDate, endDate, merchantId, cancellationToken);
        sw.Stop();
        Response.Headers.Append("X-Query-Time-Ms", sw.ElapsedMilliseconds.ToString());

        return Ok(result.Select(f => new FraudStatsDto { FraudCheckResult = f.FraudCheckResult, FraudCheckResultDisplayName = f.FraudCheckResultDisplayName, Count = f.Count, Amount = f.Amount, AvgFraudScore = f.AvgFraudScore, MaxFraudScore = f.MaxFraudScore }));
    }

    // ==========================================
    // PRIVATE HELPER
    // ==========================================
    private static TransactionDto MapToDto(TransactionReadModel model) => new()
    {
        Id = model.Id,
        ReferenceNumber = model.ReferenceNumber,
        TransactionType = model.TransactionType,
        TransactionTypeId = model.TransactionTypeId,
        TransactionTypeDisplayName = model.TransactionTypeDisplayName,
        Status = model.Status,
        StatusId = model.StatusId,
        StatusDisplayName = model.StatusDisplayName,
        Amount = model.Amount,
        Currency = model.Currency,
        AuthorizationCode = model.AuthorizationCode,
        CardNumberMasked = model.CardNumberMasked,
        MerchantId = model.MerchantId,
        MerchantCode = model.MerchantCode,
        TerminalId = model.TerminalId,
        TerminalCode = model.TerminalCode,
        DeclineReason = model.DeclineReason,
        ErrorMessage = model.ErrorMessage,
        FraudCheckResult = model.FraudCheckResult,
        FraudScore = model.FraudScore,
        OriginalTransactionId = model.OriginalTransactionId,
        SettledAt = model.SettledAt,
        BatchNumber = model.BatchNumber,
        CreatedAt = model.CreatedAt
    };
}

// Response DTOs
public class CardLimitTotalDto
{
    public string CardNumberMasked { get; set; } = null!;
    public string PeriodType { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalAmount { get; set; }
    public long QueryTimeMs { get; set; }
}

public class HourlyDistributionDto
{
    public int Hour { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class TopMerchantDto
{
    public Guid MerchantId { get; set; }
    public string MerchantCode { get; set; } = null!;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SuccessfulAmount { get; set; }
    public decimal SuccessRate { get; set; }
}

public class DeclineReasonStatsDto
{
    public string DeclineReason { get; set; } = null!;
    public string DeclineReasonDisplayName { get; set; } = null!;
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class TransactionSettlementBatchDto
{
    public string BatchNumber { get; set; } = null!;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime FirstTransaction { get; set; }
    public DateTime LastTransaction { get; set; }
    public DateTime? SettledAt { get; set; }
}

public class FraudStatsDto
{
    public string FraudCheckResult { get; set; } = null!;
    public string FraudCheckResultDisplayName { get; set; } = null!;
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public decimal AvgFraudScore { get; set; }
    public int MaxFraudScore { get; set; }
}