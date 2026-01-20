using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using BulkCardPrint.Domain.Repositories;
using BulkCardPrint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BulkCardPrint.Infrastructure.Repositories;

public class PrintBatchRepository : IPrintBatchRepository
{
    private readonly BulkCardPrintDbContext _context;

    public PrintBatchRepository(BulkCardPrintDbContext context)
    {
        _context = context;
    }

    public async Task<PrintBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PrintBatch?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .Include(x => x.Items)
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PrintBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .FirstOrDefaultAsync(x => x.BatchNumber == batchNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<PrintBatch>> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .Where(x => x.PrintVendorId == vendorId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PrintBatch>> GetByStatusAsync(PrintBatchStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PrintBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .Where(x => x.Status == PrintBatchStatus.Created ||
                        x.Status == PrintBatchStatus.FileGenerated ||
                        x.Status == PrintBatchStatus.SentToVendor ||
                        x.Status == PrintBatchStatus.InProduction)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PrintBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.PrintBatches
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PrintBatch batch, CancellationToken cancellationToken = default)
    {
        await _context.PrintBatches.AddAsync(batch, cancellationToken);
    }

    public void Update(PrintBatch batch)
    {
        _context.PrintBatches.Update(batch);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}