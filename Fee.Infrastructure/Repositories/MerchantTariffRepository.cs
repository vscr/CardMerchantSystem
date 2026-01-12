using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using Fee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Repositories;

public class MerchantTariffRepository : IMerchantTariffRepository
{
    private readonly FeeDbContext _context;

    public MerchantTariffRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantTariff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantTariffs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantTariff?> GetActiveTariffAsync(string merchantId, FeeType feeType, CancellationToken cancellationToken = default)
    {
        var tariffs = await _context.MerchantTariffs
            .Where(x => x.MerchantId == merchantId && x.IsActive)
            .ToListAsync(cancellationToken);

        return tariffs.FirstOrDefault(x => x.FeeType.Id == feeType.Id);
    }

    public async Task<IReadOnlyList<MerchantTariff>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantTariffs
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.AssignedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantTariff>> GetByTariffIdAsync(Guid tariffId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantTariffs
            .Where(x => x.TariffId == tariffId && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantTariff merchantTariff, CancellationToken cancellationToken = default)
    {
        await _context.MerchantTariffs.AddAsync(merchantTariff, cancellationToken);
    }

    public Task UpdateAsync(MerchantTariff merchantTariff, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}