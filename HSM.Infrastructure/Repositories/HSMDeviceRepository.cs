using HSM.Domain.Entities;
using HSM.Domain.Repositories;
using HSM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HSM.Infrastructure.Repositories;

public class HSMDeviceRepository : IHSMDeviceRepository
{
    private readonly HSMDbContext _context;

    public HSMDeviceRepository(HSMDbContext context)
    {
        _context = context;
    }

    public async Task<HSMDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.HSMDevices
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<HSMDevice?> GetByNameAsync(string deviceName, CancellationToken cancellationToken = default)
    {
        return await _context.HSMDevices
            .FirstOrDefaultAsync(x => x.DeviceName == deviceName, cancellationToken);
    }

    public async Task<HSMDevice?> GetPrimaryDeviceAsync(CancellationToken cancellationToken = default)
    {
        return await _context.HSMDevices
            .FirstOrDefaultAsync(x => x.IsPrimary && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<HSMDevice>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.HSMDevices
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.DeviceName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(HSMDevice device, CancellationToken cancellationToken = default)
    {
        await _context.HSMDevices.AddAsync(device, cancellationToken);
    }

    public Task UpdateAsync(HSMDevice device, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}