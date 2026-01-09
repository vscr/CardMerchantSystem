using BKM.Domain.Entities;
using BKM.Domain.Enums;
using BKM.Domain.Repositories;
using BKM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BKM.Infrastructure.Repositories;

public class SwitchMessageRepository : ISwitchMessageRepository
{
    private readonly BKMDbContext _context;

    public SwitchMessageRepository(BKMDbContext context)
    {
        _context = context;
    }

    public async Task<SwitchMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SwitchMessages
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SwitchMessage?> GetBySTANAsync(string stan, CancellationToken cancellationToken = default)
    {
        return await _context.SwitchMessages
            .FirstOrDefaultAsync(x => x.STAN == stan, cancellationToken);
    }

    public async Task<SwitchMessage?> GetByRRNAsync(string rrn, CancellationToken cancellationToken = default)
    {
        return await _context.SwitchMessages
            .FirstOrDefaultAsync(x => x.RRN == rrn, cancellationToken);
    }

    public async Task<IReadOnlyList<SwitchMessage>> GetByStatusAsync(SwitchMessageStatus status, CancellationToken cancellationToken = default)
    {
        var messages = await _context.SwitchMessages
            .ToListAsync(cancellationToken);

        return messages
            .Where(x => x.Status.Id == status.Id)
            .OrderByDescending(x => x.ReceivedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<SwitchMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.SwitchMessages
            .Where(x => x.ReceivedAt >= startDate && x.ReceivedAt <= endDate)
            .OrderByDescending(x => x.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SwitchMessage>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.SwitchMessages
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SwitchMessage message, CancellationToken cancellationToken = default)
    {
        await _context.SwitchMessages.AddAsync(message, cancellationToken);
    }

    public Task UpdateAsync(SwitchMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}