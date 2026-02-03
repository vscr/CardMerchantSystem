using Dispute.Domain.Entities;
using Dispute.Domain.Enums;
using Dispute.Domain.Repositories;
using Dispute.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dispute.Infrastructure.Repositories;

public class DisputeRepository : IDisputeRepository
{
    private readonly DisputeDbContext _context;

    public DisputeRepository(DisputeDbContext context)
    {
        _context = context;
    }

    public async Task<DisputeAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DisputeAggregate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .Include(x => x.Documents)
            .Include(x => x.Notes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DisputeAggregate?> GetByDisputeNumberAsync(string disputeNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .FirstOrDefaultAsync(x => x.DisputeNumber == disputeNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .Where(x => x.TransactionId == transactionId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetByCustomerTcknAsync(string customerTckn, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .Where(x => x.CustomerTckn == customerTckn)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default)
    {
        var disputes = await _context.Disputes
            .ToListAsync(cancellationToken);

        return disputes
            .Where(x => x.Status.Id == status.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetOverdueDisputesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var disputes = await _context.Disputes
            .Where(x => x.DueDate < now)
            .ToListAsync(cancellationToken);

        return disputes
            .Where(x => !x.Status.IsFinal)
            .OrderBy(x => x.DueDate)
            .ToList();
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetAssignedToUserAsync(string username, CancellationToken cancellationToken = default)
    {
        var disputes = await _context.Disputes
            .Where(x => x.AssignedTo == username)
            .ToListAsync(cancellationToken);

        return disputes
            .Where(x => !x.Status.IsFinal)
            .OrderByDescending(x => x.Priority.Id)
            .ThenBy(x => x.DueDate)
            .ToList();
    }

    public async Task<IReadOnlyList<DisputeAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Disputes
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<DisputeAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DisputeStatus? status = null,
        Guid? merchantId = null,
        string? customerTckn = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? isOverdue = null,
        string? assignedTo = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Temel sorgu
        var query = _context.Disputes
            .Include(x => x.Documents)
            .Include(x => x.Notes)
            .AsQueryable();

        // Merchant filtresi
        if (merchantId.HasValue)
        {
            query = query.Where(x => x.MerchantId == merchantId.Value);
        }

        // Müşteri TCKN filtresi
        if (!string.IsNullOrWhiteSpace(customerTckn))
        {
            query = query.Where(x => x.CustomerTckn == customerTckn);
        }

        // Atanan kişi filtresi
        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            query = query.Where(x => x.AssignedTo == assignedTo);
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

        // Vadesi geçmiş filtresi
        if (isOverdue.HasValue && isOverdue.Value)
        {
            query = query.Where(x => x.DueDate < now);
        }

        // Memory'de filtrelenecekler için tüm veriyi çek
        var allDisputes = await query.ToListAsync(cancellationToken);

        IEnumerable<DisputeAggregate> filteredQuery = allDisputes;

        // Status filtresi (Smart Enum - memory'de)
        if (status != null)
        {
            filteredQuery = filteredQuery.Where(x => x.Status.Id == status.Id);
        }

        // Vadesi geçmiş ama final olmayan (aktif overdue)
        if (isOverdue.HasValue && isOverdue.Value)
        {
            filteredQuery = filteredQuery.Where(x => !x.Status.IsFinal);
        }

        // Toplam sayı
        var totalCount = filteredQuery.Count();

        // Sıralama
        filteredQuery = sortBy?.ToLowerInvariant() switch
        {
            "createdat" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.CreatedAt)
                : filteredQuery.OrderBy(x => x.CreatedAt),
            "duedate" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.DueDate)
                : filteredQuery.OrderBy(x => x.DueDate),
            "amount" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.DisputedAmount)
                : filteredQuery.OrderBy(x => x.DisputedAmount),
            "priority" => sortDescending
                ? filteredQuery.OrderBy(x => x.Priority.Id)
                : filteredQuery.OrderByDescending(x => x.Priority.Id),
            "disputenumber" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.DisputeNumber)
                : filteredQuery.OrderBy(x => x.DisputeNumber),
            _ => filteredQuery.OrderByDescending(x => x.Priority.Id).ThenBy(x => x.DueDate)
        };

        // Sayfalama
        var items = filteredQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task AddAsync(DisputeAggregate dispute, CancellationToken cancellationToken = default)
    {
        await _context.Disputes.AddAsync(dispute, cancellationToken);
    }

    public Task UpdateAsync(DisputeAggregate dispute, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}