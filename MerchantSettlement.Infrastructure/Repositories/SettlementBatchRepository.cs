using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class SettlementBatchRepository : ISettlementBatchRepository
{
    private readonly MerchantSettlementDbContext _context;

    public SettlementBatchRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<SettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .FirstOrDefaultAsync(x => x.BatchNumber == batchNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetByStatusAsync(SettlementStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.PeriodStart >= startDate && x.PeriodEnd <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SettlementBatches
            .Where(x => x.Status == SettlementStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementBatch>> GetBySettlementDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.SettlementBatches
            .Where(x => x.PeriodStart >= startOfDay && x.PeriodStart < endOfDay)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SettlementBatch batch, CancellationToken cancellationToken = default)
    {
        await _context.SettlementBatches.AddAsync(batch, cancellationToken);
    }

    public void Update(SettlementBatch batch)
    {
        _context.SettlementBatches.Update(batch);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}