using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.Repositories;
using Transaction.Domain.ValueObjects;
using Transaction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Transaction.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionDbContext _context;

    public TransactionRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TransactionAggregate?> GetByReferenceNumberAsync(ReferenceNumber referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(x => x.ReferenceNumber.Value == referenceNumber.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.TerminalId == terminalId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByCardNumberAsync(string cardNumberMasked, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.CardNumberMasked == cardNumberMasked)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken = default)
    {
        var transactions = await _context.Transactions
            .ToListAsync(cancellationToken);

        return transactions
            .Where(x => x.Status.Id == status.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetPendingSettlementAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _context.Transactions
            .ToListAsync(cancellationToken);

        return transactions
            .Where(x => x.Status.Id == TransactionStatus.Approved.Id)
            .OrderBy(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<TransactionAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TransactionStatus? status = null,
        TransactionType? transactionType = null,
        Guid? merchantId = null,
        string? cardNumberMasked = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        // Temel sorgu
        var query = _context.Transactions.AsQueryable();

        // Merchant filtresi
        if (merchantId.HasValue)
        {
            query = query.Where(x => x.MerchantId == merchantId.Value);
        }

        // Kart numarası filtresi
        if (!string.IsNullOrWhiteSpace(cardNumberMasked))
        {
            query = query.Where(x => x.CardNumberMasked == cardNumberMasked);
        }

        // Tarih filtresi
        if (startDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= endDate.Value);
        }

        // Memory'de filtrelenecekler için tüm veriyi çek
        var allTransactions = await query.ToListAsync(cancellationToken);

        IEnumerable<TransactionAggregate> filteredQuery = allTransactions;

        // Status filtresi (Smart Enum - memory'de)
        if (status != null)
        {
            filteredQuery = filteredQuery.Where(x => x.Status.Id == status.Id);
        }

        // Transaction Type filtresi (Smart Enum - memory'de)
        if (transactionType != null)
        {
            filteredQuery = filteredQuery.Where(x => x.TransactionType.Id == transactionType.Id);
        }

        // Toplam sayı
        var totalCount = filteredQuery.Count();

        // Sıralama
        filteredQuery = sortBy?.ToLowerInvariant() switch
        {
            "amount" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.Amount.Amount)
                : filteredQuery.OrderBy(x => x.Amount.Amount),
            "createdat" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.CreatedAt)
                : filteredQuery.OrderBy(x => x.CreatedAt),
            "merchantcode" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.MerchantCode)
                : filteredQuery.OrderBy(x => x.MerchantCode),
            "referencenumber" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.ReferenceNumber.Value)
                : filteredQuery.OrderBy(x => x.ReferenceNumber.Value),
            _ => filteredQuery.OrderByDescending(x => x.CreatedAt)
        };

        // Sayfalama
        var items = filteredQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var transactions = await _context.Transactions
            .Where(x => x.CardNumberMasked == cardNumberMasked &&
                        x.CreatedAt >= startOfDay &&
                        x.CreatedAt < endOfDay)
            .ToListAsync(cancellationToken);

        return transactions
            .Where(x => x.Status.Id == TransactionStatus.Approved.Id ||
                        x.Status.Id == TransactionStatus.Settled.Id)
            .Where(x => x.TransactionType.DecreasesLimit)
            .Sum(x => x.Amount.Amount);
    }

    public async Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var transactions = await _context.Transactions
            .Where(x => x.CardNumberMasked == cardNumberMasked &&
                        x.CreatedAt >= startOfMonth &&
                        x.CreatedAt < endOfMonth)
            .ToListAsync(cancellationToken);

        return transactions
            .Where(x => x.Status.Id == TransactionStatus.Approved.Id ||
                        x.Status.Id == TransactionStatus.Settled.Id)
            .Where(x => x.TransactionType.DecreasesLimit)
            .Sum(x => x.Amount.Amount);
    }

    public async Task AddAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
    }

    public Task UpdateAsync(TransactionAggregate transaction, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}