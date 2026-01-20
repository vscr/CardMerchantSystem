using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;

namespace BulkCardPrint.Domain.Repositories;

public interface IPrintBatchRepository
{
    Task<PrintBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PrintBatch?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PrintBatch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintBatch>> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintBatch>> GetByStatusAsync(PrintBatchStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintBatch>> GetPendingBatchesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrintBatch>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(PrintBatch batch, CancellationToken cancellationToken = default);
    void Update(PrintBatch batch);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}