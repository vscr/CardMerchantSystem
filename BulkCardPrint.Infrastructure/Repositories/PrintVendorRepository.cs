using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;
using BulkCardPrint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BulkCardPrint.Infrastructure.Repositories;

public class PrintVendorRepository : IPrintVendorRepository
{
    private readonly BulkCardPrintDbContext _context;

    public PrintVendorRepository(BulkCardPrintDbContext context)
    {
        _context = context;
    }

    public async Task<PrintVendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrintVendors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PrintVendor?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.PrintVendors
            .FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<PrintVendor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrintVendors
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PrintVendor>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrintVendors
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrintVendor?> GetAvailableVendorAsync(int requiredCapacity, CancellationToken cancellationToken = default)
    {
        return await _context.PrintVendors
            .Where(x => x.IsActive && (x.DailyCapacity - x.CurrentDailyLoad) >= requiredCapacity)
            .OrderBy(x => x.CurrentDailyLoad)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(PrintVendor vendor, CancellationToken cancellationToken = default)
    {
        await _context.PrintVendors.AddAsync(vendor, cancellationToken);
    }

    public void Update(PrintVendor vendor)
    {
        _context.PrintVendors.Update(vendor);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}