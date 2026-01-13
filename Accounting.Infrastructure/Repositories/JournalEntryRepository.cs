using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly AccountingDbContext _context;

    public JournalEntryRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<JournalEntry?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<JournalEntry?> GetByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.EntryNumber == entryNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByPeriodAsync(string periodCode, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Where(x => x.PeriodCode == periodCode)
            .OrderByDescending(x => x.EntryDate)
            .ThenByDescending(x => x.EntryNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default)
    {
        var entries = await _context.JournalEntries
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);

        return entries.Where(x => x.Status.Id == status.Id).ToList();
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByTransactionTypeAsync(TransactionType transactionType, CancellationToken cancellationToken = default)
    {
        var entries = await _context.JournalEntries
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);

        return entries.Where(x => x.TransactionType.Id == transactionType.Id).ToList();
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByReferenceAsync(string referenceType, Guid referenceId, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Where(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Where(x => x.EntryDate >= startDate && x.EntryDate <= endDate)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        await _context.JournalEntries.AddAsync(entry, cancellationToken);
    }

    public Task UpdateAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}