using System.Data;
using CardMerchantSystem.Shared.Kernel;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Dapper tabanlı yüksek performanslı read-only repository
/// </summary>
public interface ITransactionReadRepository
{
    Task<TransactionReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransactionReadModel?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    Task<PagedResponse<TransactionReadModel>> GetPagedAsync(
        TransactionQueryFilter filter,
        CancellationToken cancellationToken = default);

    Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default);

    Task<IEnumerable<TransactionReadModel>> GetPendingSettlementAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionReadModel>> GetRecentByMerchantAsync(Guid merchantId, int limit = 100, CancellationToken cancellationToken = default);
}

/// <summary>
/// Sorgu filtresi
/// </summary>
public class TransactionQueryFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? MerchantId { get; set; }
    public Guid? TerminalId { get; set; }
    public string? CardNumberMasked { get; set; }
    public int? StatusId { get; set; }
    public int? TransactionTypeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class TransactionReadRepository : ITransactionReadRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<TransactionReadRepository> _logger;

    public TransactionReadRepository(IDapperContext context, ILogger<TransactionReadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<TransactionReadModel>(
            TransactionQueries.GetById,
            new { Id = id });
    }

    public async Task<TransactionReadModel?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<TransactionReadModel>(
            TransactionQueries.GetByReferenceNumber,
            new { ReferenceNumber = referenceNumber });
    }

    public async Task<PagedResponse<TransactionReadModel>> GetPagedAsync(
        TransactionQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Executing paged query: Page={Page}, Size={Size}, Merchant={Merchant}",
            filter.PageNumber, filter.PageSize, filter.MerchantId);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("MerchantId", filter.MerchantId);
        parameters.Add("TerminalId", filter.TerminalId);
        parameters.Add("CardNumberMasked", filter.CardNumberMasked);
        parameters.Add("StatusId", filter.StatusId);
        parameters.Add("TransactionTypeId", filter.TransactionTypeId);
        parameters.Add("StartDate", filter.StartDate);
        parameters.Add("EndDate", filter.EndDate);
        parameters.Add("MinAmount", filter.MinAmount);
        parameters.Add("MaxAmount", filter.MaxAmount);
        parameters.Add("Offset", (filter.PageNumber - 1) * filter.PageSize);
        parameters.Add("PageSize", filter.PageSize);

        // Multi-query: Count + Data in single roundtrip
        using var multi = await connection.QueryMultipleAsync(
            TransactionQueries.GetPagedTransactions,
            parameters);

        var totalCount = await multi.ReadSingleAsync<int>();
        var items = (await multi.ReadAsync<TransactionReadModel>()).ToList();

        _logger.LogDebug("Retrieved {Count} items out of {Total}", items.Count, totalCount);

        return PagedResponse<TransactionReadModel>.Create(
            items,
            totalCount,
            filter.PageNumber,
            filter.PageSize);
    }

    public async Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        var result = await connection.ExecuteScalarAsync<decimal>(
            TransactionQueries.GetDailyTotalByCard,
            new
            {
                CardNumberMasked = cardNumberMasked,
                StartOfDay = startOfDay,
                EndOfDay = endOfDay
            });

        return result;
    }

    public async Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        var result = await connection.ExecuteScalarAsync<decimal>(
            TransactionQueries.GetMonthlyTotalByCard,
            new
            {
                CardNumberMasked = cardNumberMasked,
                StartOfMonth = startOfMonth,
                EndOfMonth = endOfMonth
            });

        return result;
    }

    public async Task<IEnumerable<TransactionReadModel>> GetPendingSettlementAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TransactionReadModel>(
            TransactionQueries.GetPendingSettlement);
    }

    public async Task<IEnumerable<TransactionReadModel>> GetRecentByMerchantAsync(Guid merchantId, int limit = 100, CancellationToken cancellationToken = default)
    {
        using var connection = await _context.CreateConnectionAsync(cancellationToken);

        return await connection.QueryAsync<TransactionReadModel>(
            TransactionQueries.GetRecentByMerchant,
            new { MerchantId = merchantId, Limit = limit });
    }
}