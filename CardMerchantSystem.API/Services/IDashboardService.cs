using CardMerchantSystem.API.Models;

namespace CardMerchantSystem.API.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}