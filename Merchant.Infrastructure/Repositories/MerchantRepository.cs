using CardMerchantSystem.Shared.Kernel;
using CardMerchantSystem.Shared.Resilience;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.Repositories;
using Merchant.Domain.ValueObjects;
using Merchant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Infrastructure.Repositories;

public class MerchantRepository : IMerchantRepository
{
    private readonly MerchantDbContext _context;
    private readonly IResilientService _resilientService;

    public MerchantRepository(MerchantDbContext context, IResilientService resilientService)
    {
        _context = context;
        _resilientService = resilientService;
    }

    public async Task<MerchantAggregate?> GetByIdWithRetryAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _resilientService.ExecuteAsync(async () =>
        {
            return await _context.Merchants.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }, $"GetEntity:{id}");
    }

    public async Task<MerchantAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Merchants
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantAggregate?> GetByIdWithTerminalsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Merchants
            .Include(x => x.Terminals)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantAggregate?> GetByMerchantCodeAsync(MerchantCode code, CancellationToken cancellationToken = default)
    {
        return await _context.Merchants
            .FirstOrDefaultAsync(x => x.MerchantCode.Value == code.Value, cancellationToken);
    }

    public async Task<MerchantAggregate?> GetByTaxNumberAsync(TaxNumber taxNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Merchants
            .FirstOrDefaultAsync(x => x.TaxNumber.Value == taxNumber.Value, cancellationToken);
    }

    public async Task<bool> ExistsByTaxNumberAsync(TaxNumber taxNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Merchants
            .AnyAsync(x => x.TaxNumber.Value == taxNumber.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantAggregate>> GetByStatusAsync(MerchantStatus status, CancellationToken cancellationToken = default)
    {
        var merchants = await _context.Merchants
            .ToListAsync(cancellationToken);

        return merchants.Where(x => x.Status.Id == status.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<MerchantAggregate>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var merchants = await _context.Merchants
            .Include(x => x.Terminals)
            .ToListAsync(cancellationToken);

        return merchants.Where(x => x.Status.Id == MerchantStatus.Active.Id)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public async Task AddAsync(MerchantAggregate merchant, CancellationToken cancellationToken = default)
    {
        await _context.Merchants.AddAsync(merchant, cancellationToken);
    }

    public Task UpdateAsync(MerchantAggregate merchant, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}