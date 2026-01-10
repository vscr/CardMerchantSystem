using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HSM.Infrastructure.Repositories;

public class HSMCommandLogRepository : IHSMCommandLogRepository
{
    private readonly HSMDbContext _context;

    public HSMCommandLogRepository(HSMDbContext context)
    {
        _context = context;
    }

    public async Task<HSMCommandLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.HSMCommandLogs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<HSMCommandLog>> GetByDeviceIdAsync(Guid hsmDeviceId, CancellationToken cancellationToken = default)
    {
        return await _context.HSMCommandLogs
            .Where(x => x.HSMDeviceId == hsmDeviceId)
            .OrderByDescending(x => x.ExecutedAt)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HSMCommandLog>> GetByCommandTypeAsync(HSMCommandType commandType, CancellationToken cancellationToken = default)
    {
        var logs = await _context.HSMCommandLogs
            .OrderByDescending(x => x.ExecutedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        return logs
            .Where(x => x.CommandType.Id == commandType.Id)
            .ToList();
    }

    public async Task<IReadOnlyList<HSMCommandLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.HSMCommandLogs
            .Where(x => x.ExecutedAt >= startDate && x.ExecutedAt <= endDate)
            .OrderByDescending(x => x.ExecutedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HSMCommandLog>> GetFailedCommandsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _context.HSMCommandLogs
            .Where(x => !x.IsSuccess && x.ExecutedAt >= since)
            .OrderByDescending(x => x.ExecutedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(HSMCommandLog log, CancellationToken cancellationToken = default)
    {
        await _context.HSMCommandLogs.AddAsync(log, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}