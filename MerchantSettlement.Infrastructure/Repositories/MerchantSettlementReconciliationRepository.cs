using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class MerchantSettlementReconciliationRepository : IMerchantSettlementReconciliationRepository
{
    private readonly MerchantSettlementDbContext _context;

    public MerchantSettlementReconciliationRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantReconciliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantReconciliation?> GetByIdWithMismatchesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .Include(x => x.Mismatches)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .FirstOrDefaultAsync(x => x.ReconciliationNumber == reconciliationNumber, cancellationToken);
    }

    public async Task<MerchantReconciliation?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .FirstOrDefaultAsync(x => x.SettlementBatchId == batchId, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReconciliation>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReconciliation>> GetByStatusAsync(ReconciliationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReconciliation>> GetUnresolvedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .Where(x => x.Status != ReconciliationStatus.Matched && x.Status != ReconciliationStatus.Resolved)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReconciliation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantSettlementReconciliations
            .Where(x => x.ReconciliationDate >= startDate && x.ReconciliationDate <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantReconciliation reconciliation, CancellationToken cancellationToken = default)
    {
        await _context.MerchantSettlementReconciliations.AddAsync(reconciliation, cancellationToken);
    }

    public void Update(MerchantReconciliation reconciliation)
    {
        _context.MerchantSettlementReconciliations.Update(reconciliation);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}