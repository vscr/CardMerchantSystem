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
        // GetById tracking gerekebilir (update için), bu yüzden AsNoTracking yok
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
            .AsNoTracking()
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => x.TerminalId == terminalId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByCardNumberAsync(string cardNumberMasked, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => x.CardNumberMasked == cardNumberMasked)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken = default)
    {
        var statusId = status.Id;
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => EF.Property<int>(x, "StatusId") == statusId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetPendingSettlementAsync(CancellationToken cancellationToken = default)
    {
        var approvedStatusId = TransactionStatus.Approved.Id;
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => EF.Property<int>(x, "StatusId") == approvedStatusId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
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
        // AsNoTracking - Read-only sorgu için memory optimizasyonu
        var query = _context.Transactions.AsNoTracking().AsQueryable();

        // === TÜM FİLTRELER DB TARAFINDA ===

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

        // Status filtresi - DB'de int olarak filtreleme (Smart Enum dönüşümü configuration'da var)
        if (status != null)
        {
            var statusId = status.Id;
            query = query.Where(x => EF.Property<int>(x, "StatusId") == statusId);
        }

        // Transaction Type filtresi - DB'de int olarak filtreleme
        if (transactionType != null)
        {
            var typeId = transactionType.Id;
            query = query.Where(x => EF.Property<int>(x, "TransactionTypeId") == typeId);
        }

        // === COUNT SORGUSU (Ayrı ve optimize) ===
        var totalCount = await query.CountAsync(cancellationToken);

        // === SIRALAMA (DB tarafında) ===
        IOrderedQueryable<TransactionAggregate> orderedQuery = sortBy?.ToLowerInvariant() switch
        {
            "amount" => sortDescending
                ? query.OrderByDescending(x => x.Amount.Amount)
                : query.OrderBy(x => x.Amount.Amount),
            "createdat" => sortDescending
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt),
            "merchantcode" => sortDescending
                ? query.OrderByDescending(x => x.MerchantCode)
                : query.OrderBy(x => x.MerchantCode),
            "referencenumber" => sortDescending
                ? query.OrderByDescending(x => x.ReferenceNumber.Value)
                : query.OrderBy(x => x.ReferenceNumber.Value),
            "status" => sortDescending
                ? query.OrderByDescending(x => EF.Property<int>(x, "StatusId"))
                : query.OrderBy(x => EF.Property<int>(x, "StatusId")),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        // === SAYFALAMA (DB tarafında - OFFSET/FETCH) ===
        var items = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<decimal> GetDailyTotalByCardAsync(string cardNumberMasked, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        // Status: Approved(2), Settled(7)
        var validStatusIds = new[] { TransactionStatus.Approved.Id, TransactionStatus.Settled.Id };

        // TransactionType: Sale(1), PreAuth(4), CashAdvance(6) - DecreasesLimit = true
        var decreasesLimitTypeIds = new[] { TransactionType.Sale.Id, TransactionType.PreAuth.Id, TransactionType.CashAdvance.Id };

        // Tüm filtreleme ve SUM DB tarafında
        var total = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.CardNumberMasked == cardNumberMasked &&
                        x.CreatedAt >= startOfDay &&
                        x.CreatedAt < endOfDay &&
                        validStatusIds.Contains(EF.Property<int>(x, "StatusId")) &&
                        decreasesLimitTypeIds.Contains(EF.Property<int>(x, "TransactionTypeId")))
            .SumAsync(x => x.Amount.Amount, cancellationToken);

        return total;
    }

    public async Task<decimal> GetMonthlyTotalByCardAsync(string cardNumberMasked, int year, int month, CancellationToken cancellationToken = default)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        // Status: Approved(2), Settled(7)
        var validStatusIds = new[] { TransactionStatus.Approved.Id, TransactionStatus.Settled.Id };

        // TransactionType: Sale(1), PreAuth(4), CashAdvance(6) - DecreasesLimit = true
        var decreasesLimitTypeIds = new[] { TransactionType.Sale.Id, TransactionType.PreAuth.Id, TransactionType.CashAdvance.Id };

        // Tüm filtreleme ve SUM DB tarafında
        var total = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.CardNumberMasked == cardNumberMasked &&
                        x.CreatedAt >= startOfMonth &&
                        x.CreatedAt < endOfMonth &&
                        validStatusIds.Contains(EF.Property<int>(x, "StatusId")) &&
                        decreasesLimitTypeIds.Contains(EF.Property<int>(x, "TransactionTypeId")))
            .SumAsync(x => x.Amount.Amount, cancellationToken);

        return total;
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