using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class MerchantSettlementBatchRepository : IMerchantSettlementBatchRepository
{
    private readonly MerchantSettlementDbContext _context;

    public MerchantSettlementBatchRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantSettlementBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantSettlementBatch?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantSettlementBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .FirstOrDefaultAsync(x => x.BatchNumber == batchNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantSettlementBatch>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantSettlementBatch>> GetByStatusAsync(SettlementStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantSettlementBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .Where(x => x.PeriodStart >= startDate && x.PeriodEnd <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantSettlementBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementBatches
            .Where(x => x.Status == SettlementStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantSettlementBatch>> GetBySettlementDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.MerchantSettlementBatches
            .Where(x => x.PeriodStart >= startOfDay && x.PeriodStart < endOfDay)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantSettlementBatch batch, CancellationToken cancellationToken = default)
    {
        await _context.MerchantSettlementBatches.AddAsync(batch, cancellationToken);
    }

    public void Update(MerchantSettlementBatch batch)
    {
        _context.MerchantSettlementBatches.Update(batch);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}