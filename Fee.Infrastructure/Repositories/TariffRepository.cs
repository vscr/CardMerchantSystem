using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using Fee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Repositories;

public class TariffRepository : ITariffRepository
{
    private readonly FeeDbContext _context;

    public TariffRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<Tariff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tariffs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Tariff?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tariffs
            .Include(x => x.Rules)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Tariff?> GetByCodeAsync(string tariffCode, CancellationToken cancellationToken = default)
    {
        return await _context.Tariffs
            .Include(x => x.Rules)
            .FirstOrDefaultAsync(x => x.TariffCode == tariffCode, cancellationToken);
    }

    public async Task<Tariff?> GetDefaultTariffAsync(FeeType feeType, CancellationToken cancellationToken = default)
    {
        var tariffs = await _context.Tariffs
            .Include(x => x.Rules)
            .Where(x => x.IsDefault)
            .ToListAsync(cancellationToken);

        return tariffs.FirstOrDefault(x => x.FeeType.Id == feeType.Id && x.Status.IsUsable);
    }

    public async Task<IReadOnlyList<Tariff>> GetActiveByFeeTypeAsync(FeeType feeType, CancellationToken cancellationToken = default)
    {
        var tariffs = await _context.Tariffs
            .Include(x => x.Rules)
            .ToListAsync(cancellationToken);

        return tariffs
            .Where(x => x.FeeType.Id == feeType.Id && x.Status.IsUsable)
            .OrderBy(x => x.TariffCode)
            .ToList();
    }

    public async Task<IReadOnlyList<Tariff>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var tariffs = await _context.Tariffs
            .Include(x => x.Rules)
            .ToListAsync(cancellationToken);

        return tariffs
            .Where(x => x.Status.IsUsable || x.Status == TariffStatus.Draft)
            .OrderBy(x => x.TariffCode)
            .ToList();
    }

    public async Task AddAsync(Tariff tariff, CancellationToken cancellationToken = default)
    {
        await _context.Tariffs.AddAsync(tariff, cancellationToken);
    }

    public Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}