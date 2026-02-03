using Dapper;
using Microsoft.Extensions.Logging;
using Transaction.Domain.Enums;

namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Raporlama ve istatistik sorguları için Dapper repository
/// </summary>
public interface ITransactionReportRepository
{
    Task<TransactionStatsReadDto> GetStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TransactionTypeStatsReadDto>> GetStatsByTransactionTypeAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DailyTrendReadDto>> GetDailyTrendAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HourlyDistributionReadDto>> GetHourlyDistributionAsync(
        DateTime date,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TopMerchantReadDto>> GetTopMerchantsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DeclineReasonStatsReadDto>> GetDeclineReasonStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SettlementBatchReadDto>> GetSettlementBatchSummaryAsync(
        string? batchNumber = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FraudStatsReadDto>> GetFraudStatsAsync(
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

    public async Task<TransactionStatsReadDto> GetStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Getting transaction stats from {Start} to {End}, Merchant={Merchant}",
            startDate, endDate, merchantId);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        var stats = await connection.QuerySingleAsync<TransactionStatsReadDto>(
            ReportQueries.GetTransactionStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });

        return stats;
    }

    public async Task<IEnumerable<TransactionTypeStatsReadDto>> GetStatsByTransactionTypeAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TransactionTypeStatsReadDto>(
            ReportQueries.GetStatsByTransactionType,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<DailyTrendReadDto>> GetDailyTrendAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<DailyTrendReadDto>(
            ReportQueries.GetDailyTrend,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<HourlyDistributionReadDto>> GetHourlyDistributionAsync(
        DateTime date,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<HourlyDistributionReadDto>(
            ReportQueries.GetHourlyDistribution,
            new { StartOfDay = startOfDay, EndOfDay = endOfDay, MerchantId = merchantId });
    }

    public async Task<IEnumerable<TopMerchantReadDto>> GetTopMerchantsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TopMerchantReadDto>(
            ReportQueries.GetTopMerchants,
            new { StartDate = startDate, EndDate = endDate, Limit = limit });
    }

    public async Task<IEnumerable<DeclineReasonStatsReadDto>> GetDeclineReasonStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<DeclineReasonStatsReadDto>(
            ReportQueries.GetDeclineReasonStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }

    public async Task<IEnumerable<SettlementBatchReadDto>> GetSettlementBatchSummaryAsync(
        string? batchNumber = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<SettlementBatchReadDto>(
            ReportQueries.GetSettlementBatchSummary,
            new { BatchNumber = batchNumber, StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<FraudStatsReadDto>> GetFraudStatsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? merchantId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<FraudStatsReadDto>(
            ReportQueries.GetFraudStats,
            new { StartDate = startDate, EndDate = endDate, MerchantId = merchantId });
    }
}