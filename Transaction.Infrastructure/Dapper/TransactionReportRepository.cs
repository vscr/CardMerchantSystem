using Dapper;
using Microsoft.Extensions.Logging;

namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Raporlama ve istatistik sorguları için Dapper repository
/// </summary>
public interface ITransactionReportRepository
{
    Task<TransactionStatsReadModel> GetStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TransactionTypeStatsReadModel>> GetStatsByTransactionTypeAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DailyTrendReadModel>> GetDailyTrendAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HourlyDistributionReadModel>> GetHourlyDistributionAsync(
        DateTime date,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TopMerchantReadModel>> GetTopMerchantsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DeclineReasonStatsReadModel>> GetDeclineReasonStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SettlementBatchReadModel>> GetSettlementBatchSummaryAsync(
        string? batchNumber = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FraudStatsReadModel>> GetFraudStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);
}

public class TransactionReportRepository : ITransactionReportRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<TransactionReportRepository> _logger;

    public TransactionReportRepository(IDapperContext context, ILogger<TransactionReportRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionStatsReadModel> GetStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Getting transaction stats from {Start} to {End}, Merchant={Merchant}",
            startDate, endDate, merchantId);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        var stats = await connection.QuerySingleAsync<TransactionStatsReadModel>(
            ReportQueries.GetTransactionStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });

        return stats;
    }

    public async Task<IEnumerable<TransactionTypeStatsReadModel>> GetStatsByTransactionTypeAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TransactionTypeStatsReadModel>(
            ReportQueries.GetStatsByTransactionType,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<DailyTrendReadModel>> GetDailyTrendAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<DailyTrendReadModel>(
            ReportQueries.GetDailyTrend,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<HourlyDistributionReadModel>> GetHourlyDistributionAsync(
        DateTime date,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<HourlyDistributionReadModel>(
            ReportQueries.GetHourlyDistribution,
            new { StartOfDay = startOfDay, EndOfDay = endOfDay, MerchantId = merchantId });
    }

    public async Task<IEnumerable<TopMerchantReadModel>> GetTopMerchantsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TopMerchantReadModel>(
            ReportQueries.GetTopMerchants,
            new { StartDate = startDate, EndDate = endDate, Limit = limit });
    }

    public async Task<IEnumerable<DeclineReasonStatsReadModel>> GetDeclineReasonStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<DeclineReasonStatsReadModel>(
            ReportQueries.GetDeclineReasonStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<SettlementBatchReadModel>> GetSettlementBatchSummaryAsync(
        string? batchNumber = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<SettlementBatchReadModel>(
            ReportQueries.GetSettlementBatchSummary,
            new { BatchNumber = batchNumber, StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<FraudStatsReadModel>> GetFraudStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<FraudStatsReadModel>(
            ReportQueries.GetFraudStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }
}