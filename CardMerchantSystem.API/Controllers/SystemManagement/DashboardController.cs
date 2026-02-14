using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Models;
using CardMerchantSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CardMerchantSystem.API.Controllers.SystemManagement;

[Authorize(Policy = Policies.ViewerOrAbove)]
[EnableRateLimiting("Relaxed")]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Dashboard istatistiklerini getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Sadece kart istatistiklerini getirir
    /// </summary>
    [HttpGet("cards")]
    public async Task<ActionResult<CardStats>> GetCardStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result.Cards);
    }

    /// <summary>
    /// Sadece işlem istatistiklerini getirir
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<TransactionStats>> GetTransactionStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result.Transactions);
    }

    /// <summary>
    /// Sadece bloke istatistiklerini getirir
    /// </summary>
    [HttpGet("blocks")]
    public async Task<ActionResult<BlockStats>> GetBlockStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result.Blocks);
    }

    /// <summary>
    /// Sadece iş emri istatistiklerini getirir
    /// </summary>
    [HttpGet("workorders")]
    public async Task<ActionResult<WorkOrderStats>> GetWorkOrderStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result.WorkOrders);
    }

    /// <summary>
    /// Sadece üye işyeri istatistiklerini getirir
    /// </summary>
    [HttpGet("merchants")]
    public async Task<ActionResult<MerchantStats>> GetMerchantStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(result.Merchants);
    }
}