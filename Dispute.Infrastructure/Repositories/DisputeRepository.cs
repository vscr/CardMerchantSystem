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