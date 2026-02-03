using Campaign.Domain.Entities;
using Campaign.Domain.Enums;

namespace Campaign.Domain.Repositories;

/// <summary>
/// Kampanya repository interface
/// </summary>
public interface ICampaignRepository
{
    Task<CampaignAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CampaignAggregate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CampaignAggregate?> GetByCampaignCodeAsync(string campaignCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignAggregate>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignAggregate>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignAggregate>> GetExpiredCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sayfalı kampanya listesi getirir
    /// </summary>
    Task<(IReadOnlyList<CampaignAggregate> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default);

    Task<int> GetUsageCountByCustomerAsync(Guid campaignId, string cardNumberMasked, CancellationToken cancellationToken = default);

    Task AddAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default);

    Task UpdateAsync(CampaignAggregate campaign, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}