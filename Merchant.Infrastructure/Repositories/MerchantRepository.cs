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
    private readonly MerchantDbContextBase _context;
    private readonly IResilientService _resilientService;

    public MerchantRepository(MerchantDbContextBase context, IResilientService resilientService)
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

    public async Task<(IReadOnlyList<MerchantAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        MerchantStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Merchants.AsQueryable();

        // Status filtresi - Smart Enum olduğu için memory'de filtrelememiz gerekiyor
        var allMerchants = await query.ToListAsync(cancellationToken);

        IEnumerable<MerchantAggregate> filteredQuery = allMerchants;

        // Status filtresi
        if (status != null)
        {
            filteredQuery = filteredQuery.Where(x => x.Status.Id == status.Id);
        }

        // Arama filtresi
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            filteredQuery = filteredQuery.Where(x =>
                x.Name.ToLowerInvariant().Contains(term) ||
                x.TradeName.ToLowerInvariant().Contains(term) ||
                x.MerchantCode.Value.ToLowerInvariant().Contains(term) ||
                x.Email.ToLowerInvariant().Contains(term));
        }

        // Toplam sayı
        var totalCount = filteredQuery.Count();

        // Sıralama
        filteredQuery = sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.Name)
                : filteredQuery.OrderBy(x => x.Name),
            "createdat" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.CreatedAt)
                : filteredQuery.OrderBy(x => x.CreatedAt),
            "merchantcode" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.MerchantCode.Value)
                : filteredQuery.OrderBy(x => x.MerchantCode.Value),
            _ => filteredQuery.OrderByDescending(x => x.CreatedAt)
        };

        // Sayfalama
        var items = filteredQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
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