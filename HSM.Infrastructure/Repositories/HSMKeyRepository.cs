using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HSM.Infrastructure.Repositories;

public class HSMKeyRepository : IHSMKeyRepository
{
    private readonly HSMDbContext _context;

    public HSMKeyRepository(HSMDbContext context)
    {
        _context = context;
    }

    public async Task<HSMKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.HSMKeys
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<HSMKey?> GetByNameAsync(string keyName, CancellationToken cancellationToken = default)
    {
        return await _context.HSMKeys
            .FirstOrDefaultAsync(x => x.KeyName == keyName, cancellationToken);
    }

    public async Task<HSMKey?> GetByIndexAsync(string keyIndex, CancellationToken cancellationToken = default)
    {
        return await _context.HSMKeys
            .FirstOrDefaultAsync(x => x.KeyIndex == keyIndex && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<HSMKey>> GetByTypeAsync(HSMKeyType keyType, CancellationToken cancellationToken = default)
    {
        var keys = await _context.HSMKeys
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        return keys
            .Where(x => x.KeyType.Id == keyType.Id)
            .OrderBy(x => x.KeyName)
            .ToList();
    }

    public async Task<IReadOnlyList<HSMKey>> GetByDeviceIdAsync(Guid hsmDeviceId, CancellationToken cancellationToken = default)
    {
        return await _context.HSMKeys
            .Where(x => x.HSMDeviceId == hsmDeviceId && x.IsActive)
            .OrderBy(x => x.KeyName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HSMKey>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.HSMKeys
            .Where(x => x.IsActive)
            .OrderBy(x => x.KeyName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(HSMKey key, CancellationToken cancellationToken = default)
    {
        await _context.HSMKeys.AddAsync(key, cancellationToken);
    }

    public Task UpdateAsync(HSMKey key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}