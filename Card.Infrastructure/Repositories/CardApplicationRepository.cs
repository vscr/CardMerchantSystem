using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.Repositories;
using Card.Domain.ValueObjects;
using Card.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Card.Infrastructure.Repositories;

public class CardApplicationRepository : ICardApplicationRepository
{
    private readonly CardDbContext _context;

    public CardApplicationRepository(CardDbContext context)
    {
        _context = context;
    }

    public async Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardApplication?> GetByIdWithHistoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CardApplication>> GetByCustomerTcknAsync(TCKN tckn, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.CustomerTckn == tckn)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveApplicationAsync(TCKN tckn, CancellationToken cancellationToken = default)
    {
        // Aktif = Pending, UnderReview, Approved, CardRequested, CardPrinted, ReadyForDelivery, InDelivery
        var activeStatuses = new[]
        {
            CardApplicationStatus.Pending.Id,
            CardApplicationStatus.UnderReview.Id,
            CardApplicationStatus.Approved.Id,
            CardApplicationStatus.CardRequested.Id,
            CardApplicationStatus.CardPrinted.Id,
            CardApplicationStatus.ReadyForDelivery.Id,
            CardApplicationStatus.InDelivery.Id
        };

        return await _context.CardApplications
            .AnyAsync(x => x.CustomerTckn == tckn && activeStatuses.Contains(x.Status.Id), cancellationToken);
    }

    public async Task<IReadOnlyList<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardApplication>> GetByPrintBatchIdAsync(string batchId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(x => x.PrintBatchId == batchId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        await _context.CardApplications.AddAsync(application, cancellationToken);
    }

    public Task UpdateAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        _context.CardApplications.Update(application);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}