using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using EarlyBlockResolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyBlockResolution.Infrastructure.Repositories;

public class CardBlockRepository : ICardBlockRepository
{
    private readonly EarlyBlockResolutionDbContext _context;

    public CardBlockRepository(EarlyBlockResolutionDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CardBlock>> GetActiveBlocksByCardMaskedAsync(string cardNumberMasked, CancellationToken cancellationToken = default)
    => await _context.CardBlocks
        .AsNoTracking()
       .Where(x => x.CardNumberMasked == cardNumberMasked
         && (x.Status == BlockStatus.Active || x.Status == BlockStatus.PendingVerification))
        .ToListAsync(cancellationToken);

    public async Task<CardBlock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardBlock?> GetByIdWithVerificationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardBlock?> GetByBlockNumberAsync(string blockNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .FirstOrDefaultAsync(x => x.BlockNumber == blockNumber, cancellationToken);
    }

    public async Task<CardBlock?> GetActiveBlockByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.CardId == cardId &&
                        (x.Status == BlockStatus.Active || x.Status == BlockStatus.PendingVerification))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardBlock>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.CardId == cardId)
            .OrderByDescending(x => x.BlockedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardBlock>> GetByStatusAsync(BlockStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.BlockedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardBlock>> GetPendingVerificationAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.Status == BlockStatus.PendingVerification)
            .OrderBy(x => x.BlockedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardBlock>> GetExpiredBlocksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.ExpiresAt.HasValue &&
                        x.ExpiresAt <= now &&
                        x.Status != BlockStatus.Expired &&
                        x.Status != BlockStatus.Resolved &&
                        x.Status != BlockStatus.PermanentBlock)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardBlock>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Include(x => x.Verifications)
            .Where(x => x.BlockedAt >= startDate && x.BlockedAt <= endDate)
            .OrderByDescending(x => x.BlockedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CardBlock block, CancellationToken cancellationToken = default)
    {
        await _context.CardBlocks.AddAsync(block, cancellationToken);
    }

    public void Update(CardBlock block)
    {
        _context.CardBlocks.Update(block);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}