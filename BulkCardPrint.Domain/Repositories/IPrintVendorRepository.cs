using BulkCardPrint.Domain.Entities;

namespace BulkCardPrint.Domain.Repositories;

public interface IPrintVendorRepository
{
    Task<PrintVendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PrintVendor?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintVendor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintVendor>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<PrintVendor?> GetAvailableVendorAsync(int requiredCapacity, CancellationToken cancellationToken = default);
    Task AddAsync(PrintVendor vendor, CancellationToken cancellationToken = default);
    void Update(PrintVendor vendor);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}