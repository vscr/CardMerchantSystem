using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Repositories;

/// <summary>
/// Muhasebe Fişi Repository Interface
/// </summary>
public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByPeriodAsync(string periodCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByTransactionTypeAsync(TransactionType transactionType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByReferenceAsync(string referenceType, Guid referenceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntry entry, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntry entry, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}