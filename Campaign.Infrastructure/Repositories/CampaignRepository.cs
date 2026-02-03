using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Campaign.Domain.Repositories;
using Campaign.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly CampaignDbContext _context;

    public CampaignRepository(CampaignDbContext context)
    {
        _context = context;
    }

    public async Task<CampaignAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CampaignAggregate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .Include(x => x.Rules)
            .Include(x => x.Usages)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CampaignAggregate?> GetByCampaignCodeAsync(string campaignCode, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .Include(x => x.Rules)
            .FirstOrDefaultAsync(x => x.CampaignCode == campaignCode, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignAggregate>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
    {
        var campaigns = await _context.Campaigns
            .ToListAsync(cancellationToken);

        return campaigns
            .Where(x => x.Status.Id == status.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<CampaignAggregate>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var campaigns = await _context.Campaigns
            .Where(x => x.StartDate <= now && x.EndDate >= now)
            .ToListAsync(cancellationToken);

        return campaigns
            .Where(x => x.Status.IsUsable)
            .OrderBy(x => x.EndDate)
            .ToList();
    }

    public async Task<IReadOnlyList<CampaignAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        var campaigns = await _context.Campaigns
            .Where(x => x.IsAllMerchants ||
                        (x.AllowedMerchantIds != null && x.AllowedMerchantIds.Contains(merchantId)))
            .ToListAsync(cancellationToken);

        return campaigns
            .Where(x => x.Status.IsUsable)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<CampaignAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignAggregate>> GetExpiredCampaignsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var campaigns = await _context.Campaigns
            .Where(x => x.EndDate < now)
            .ToListAsync(cancellationToken);

        return campaigns
            .Where(x => !x.Status.IsFinal)
            .OrderBy(x => x.EndDate)
            .ToList();
    }

    public async Task<(IReadOnlyList<CampaignAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CampaignStatus? status = null,
        Guid? merchantId = null,
        string? searchTerm = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        bool? isActive = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Temel sorgu
        var query = _context.Campaigns
            .Include(x => x.Rules)
            .AsQueryable();

        // Merchant filtresi
        if (merchantId.HasValue)
        {
            query = query.Where(x => x.IsAllMerchants ||
                (x.AllowedMerchantIds != null && x.AllowedMerchantIds.Contains(merchantId.Value)));
        }

        // Tarih filtresi
        if (startDateFrom.HasValue)
        {
            query = query.Where(x => x.StartDate >= startDateFrom.Value);
        }

        if (startDateTo.HasValue)
        {
            query = query.Where(x => x.StartDate <= startDateTo.Value);
        }

        // Arama filtresi
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) ||
                x.CampaignCode.ToLower().Contains(term) ||
                x.Description.ToLower().Contains(term));
        }

        // Memory'de filtrelenecekler için tüm veriyi çek
        var allCampaigns = await query.ToListAsync(cancellationToken);

        IEnumerable<CampaignAggregate> filteredQuery = allCampaigns;

        // Status filtresi (Smart Enum - memory'de)
        if (status != null)
        {
            filteredQuery = filteredQuery.Where(x => x.Status.Id == status.Id);
        }

        // Aktif kampanya filtresi
        if (isActive.HasValue && isActive.Value)
        {
            filteredQuery = filteredQuery.Where(x =>
                x.Status.IsUsable &&
                x.StartDate <= now &&
                x.EndDate >= now);
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
            "startdate" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.StartDate)
                : filteredQuery.OrderBy(x => x.StartDate),
            "enddate" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.EndDate)
                : filteredQuery.OrderBy(x => x.EndDate),
            "usagecount" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.CurrentUsageCount)
                : filteredQuery.OrderBy(x => x.CurrentUsageCount),
            "campaigncode" => sortDescending
                ? filteredQuery.OrderByDescending(x => x.CampaignCode)
                : filteredQuery.OrderBy(x => x.CampaignCode),
            _ => filteredQuery.OrderByDescending(x => x.CreatedAt)
        };

        // Sayfalama
        var items = filteredQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task<int> GetUsageCountByCustomerAsync(Guid campaignId, string cardNumberMasked, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignUsages
            .CountAsync(x => x.CampaignId == campaignId && x.CardNumberMasked == cardNumberMasked, cancellationToken);
    }

    public async Task AddAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default)
    {
        await _context.Campaigns.AddAsync(campaign, cancellationToken);
    }

    public Task UpdateAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}