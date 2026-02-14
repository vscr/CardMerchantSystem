using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.Repositories;
using Card.Domain.ValueObjects;
using Card.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Card.Infrastructure.Repositories;

public class CardApplicationRepository : ICardApplicationRepository
{
    private readonly CardDbContext _context;

    public CardApplicationRepository(CardDbContext context)
    {
        _context = context;
    }
    public async Task<CardApplication?> GetByMaskedCardNoAsync(string maskedCardNo, CancellationToken cancellationToken = default)
    => await _context.CardApplications
        .AsNoTracking()
        .Where(x => x.CardNumberMasked == maskedCardNo && x.Status == CardApplicationStatus.Approved)
        .OrderByDescending(x => x.CreatedAt)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<(IReadOnlyList<CardApplication> Items, int TotalCount)> GetPagedAsync(
    int pageNumber,
    int pageSize,
    int? statusId = null,
    int? cardTypeId = null,
    string? customerTckn = null,
    string? customerName = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? sortBy = null,
    bool sortDescending = false,
    CancellationToken cancellationToken = default)
    {
        var query = _context.CardApplications.AsQueryable();

        // Filters
        if (statusId.HasValue)
            query = query.Where(x => x.Status.Id == statusId.Value);

        if (cardTypeId.HasValue)
            query = query.Where(x => x.CardType.Id == cardTypeId.Value);

        if (!string.IsNullOrWhiteSpace(customerTckn))
            query = query.Where(x => x.CustomerTckn.Value == customerTckn);

        if (!string.IsNullOrWhiteSpace(customerName))
            query = query.Where(x => (x.CustomerName + " " + x.CustomerSurname).Contains(customerName));

        if (startDate.HasValue)
            query = query.Where(x => x.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(x => x.CreatedAt <= endDate.Value);

        // Total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "createdat" => sortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            "customername" => sortDescending ? query.OrderByDescending(x => x.CustomerName) : query.OrderBy(x => x.CustomerName),
            "status" => sortDescending ? query.OrderByDescending(x => x.Status.Id) : query.OrderBy(x => x.Status.Id),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        // Pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<CardApplication>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    public async Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardApplication?> GetByIdWithHistoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CardApplication>> GetByCustomerTcknAsync(TCKN tckn, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.CustomerTckn == tckn)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveApplicationAsync(TCKN tckn, CancellationToken cancellationToken = default)
    {
        var applications = await _context.CardApplications
            .Where(x => x.CustomerTckn.Value == tckn.Value)
            .ToListAsync(cancellationToken);

        var finalStatusIds = new[]
        {
        CardApplicationStatus.Delivered.Id,
        CardApplicationStatus.Rejected.Id,
        CardApplicationStatus.Cancelled.Id
    };

        return applications.Any(x => !finalStatusIds.Contains(x.Status.Id));
    }

    public async Task<IReadOnlyList<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardApplication>> GetByPrintBatchIdAsync(string batchId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.PrintBatchId == batchId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        await _context.CardApplications.AddAsync(application, cancellationToken);
    }

    public Task UpdateAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        _context.CardApplications.Update(application);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}