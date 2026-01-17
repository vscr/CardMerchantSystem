using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Repositories;

public class SettlementReconciliationRepository : ISettlementReconciliationRepository
{
    private readonly MerchantSettlementDbContext _context;

    public SettlementReconciliationRepository(MerchantSettlementDbContext context)
    {
        _context = context;
    }

    public async Task<SettlementReconciliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementReconciliation?> GetByIdWithMismatchesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .Include(x => x.Mismatches)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SettlementReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .FirstOrDefaultAsync(x => x.ReconciliationNumber == reconciliationNumber, cancellationToken);
    }

    public async Task<SettlementReconciliation?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .FirstOrDefaultAsync(x => x.SettlementBatchId == batchId, cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementReconciliation>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementReconciliation>> GetByStatusAsync(ReconciliationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementReconciliation>> GetUnresolvedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .Where(x => x.Status != ReconciliationStatus.Matched && x.Status != ReconciliationStatus.Resolved)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementReconciliation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.SettlementReconciliations
            .Where(x => x.ReconciliationDate >= startDate && x.ReconciliationDate <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SettlementReconciliation reconciliation, CancellationToken cancellationToken = default)
    {
        await _context.SettlementReconciliations.AddAsync(reconciliation, cancellationToken);
    }

    public void Update(SettlementReconciliation reconciliation)
    {
        _context.SettlementReconciliations.Update(reconciliation);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}