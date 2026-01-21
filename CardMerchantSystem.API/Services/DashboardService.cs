using CardMerchantSystem.API.Models;
using Card.Infrastructure.Persistence;
using Transaction.Infrastructure.Persistence;
using EarlyBlockResolution.Infrastructure.Persistence;
using WorkOrder.Infrastructure.Persistence;
using Merchant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EarlyBlockResolution.Domain.Enums;
using WorkOrder.Domain.Enums;
using Merchant.Domain.Enums;
using Card.Domain.Enums;
using Transaction.Domain.Enums;

namespace CardMerchantSystem.API.Services;

public class DashboardService : IDashboardService
{
    private readonly CardDbContext _cardDb;
    private readonly TransactionDbContext _transactionDb;
    private readonly EarlyBlockResolutionDbContext _blockDb;
    private readonly WorkOrderDbContext _workOrderDb;
    private readonly MerchantDbContext _merchantDb;

    public DashboardService(
        CardDbContext cardDb,
        TransactionDbContext transactionDb,
        EarlyBlockResolutionDbContext blockDb,
        WorkOrderDbContext workOrderDb,
        MerchantDbContext merchantDb)
    {
        _cardDb = cardDb;
        _transactionDb = transactionDb;
        _blockDb = blockDb;
        _workOrderDb = workOrderDb;
        _merchantDb = merchantDb;
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var dashboard = new DashboardDto
        {
            Cards = await GetCardStatsAsync(today, cancellationToken),
            Transactions = await GetTransactionStatsAsync(today, cancellationToken),
            Blocks = await GetBlockStatsAsync(today, cancellationToken),
            WorkOrders = await GetWorkOrderStatsAsync(today, cancellationToken),
            Merchants = await GetMerchantStatsAsync(cancellationToken),
            GeneratedAt = DateTime.UtcNow
        };

        return dashboard;
    }

    private async Task<CardStats> GetCardStatsAsync(DateTime today, CancellationToken ct)
    {
        // CardApplication tablosundan istatistikler
        var totalApplications = await _cardDb.CardApplications.CountAsync(ct);

        var deliveredCount = await _cardDb.CardApplications
            .CountAsync(c => c.Status == CardApplicationStatus.Delivered, ct);

        var blockedCount = await _cardDb.CardApplications
            .CountAsync(c => c.Status == CardApplicationStatus.Cancelled ||
                            c.Status == CardApplicationStatus.Rejected, ct);

        var todayApplications = await _cardDb.CardApplications
            .CountAsync(a => a.CreatedAt >= today, ct);

        var pendingApplications = await _cardDb.CardApplications
            .CountAsync(a => a.Status == CardApplicationStatus.Pending, ct);

        return new CardStats
        {
            TotalCards = deliveredCount,
            ActiveCards = deliveredCount,
            BlockedCards = blockedCount,
            TodayApplications = todayApplications,
            PendingApplications = pendingApplications
        };
    }

    private async Task<TransactionStats> GetTransactionStatsAsync(DateTime today, CancellationToken ct)
    {
        var todayTransactions = await _transactionDb.Transactions
            .Where(t => t.CreatedAt >= today)
            .ToListAsync(ct);

        var todayCount = todayTransactions.Count;
        var todayAmount = todayTransactions.Sum(t => t.Amount.Amount);
        var todaySuccessful = todayTransactions.Count(t => t.Status == TransactionStatus.Approved ||
                                                          t.Status == TransactionStatus.Settled);
        var todayFailed = todayTransactions.Count(t => t.Status == TransactionStatus.Declined);

        return new TransactionStats
        {
            TodayCount = todayCount,
            TodayAmount = todayAmount,
            TodaySuccessful = todaySuccessful,
            TodayFailed = todayFailed,
            TodaySuccessRate = todayCount > 0 ? Math.Round((decimal)todaySuccessful / todayCount * 100, 2) : 0
        };
    }

    private async Task<BlockStats> GetBlockStatsAsync(DateTime today, CancellationToken ct)
    {
        var activeBlocks = await _blockDb.CardBlocks
            .CountAsync(b => b.Status == BlockStatus.Active, ct);

        var pendingVerification = await _blockDb.CardBlocks
            .CountAsync(b => b.Status == BlockStatus.PendingVerification, ct);

        var todayNewBlocks = await _blockDb.CardBlocks
            .CountAsync(b => b.BlockedAt >= today, ct);

        var todayResolved = await _blockDb.CardBlocks
            .CountAsync(b => b.ResolvedAt != null && b.ResolvedAt >= today, ct);

        return new BlockStats
        {
            ActiveBlocks = activeBlocks,
            PendingVerification = pendingVerification,
            TodayNewBlocks = todayNewBlocks,
            TodayResolved = todayResolved
        };
    }

    private async Task<WorkOrderStats> GetWorkOrderStatsAsync(DateTime today, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var openOrders = await _workOrderDb.WorkOrderItems
            .CountAsync(w => w.Status == WorkOrderStatus.Open ||
                            w.Status == WorkOrderStatus.InProgress ||
                            w.Status == WorkOrderStatus.PendingApproval ||
                            w.Status == WorkOrderStatus.OnHold, ct);

        var overdueOrders = await _workOrderDb.WorkOrderItems
            .CountAsync(w => w.DueDate < now &&
                            (w.Status == WorkOrderStatus.Open ||
                             w.Status == WorkOrderStatus.InProgress ||
                             w.Status == WorkOrderStatus.PendingApproval ||
                             w.Status == WorkOrderStatus.OnHold), ct);

        var todayCreated = await _workOrderDb.WorkOrderItems
            .CountAsync(w => w.CreatedAt >= today, ct);

        var todayCompleted = await _workOrderDb.WorkOrderItems
            .CountAsync(w => w.CompletedAt != null && w.CompletedAt >= today, ct);

        return new WorkOrderStats
        {
            OpenOrders = openOrders,
            OverdueOrders = overdueOrders,
            TodayCreated = todayCreated,
            TodayCompleted = todayCompleted
        };
    }

    private async Task<MerchantStats> GetMerchantStatsAsync(CancellationToken ct)
    {
        var totalMerchants = await _merchantDb.Merchants.CountAsync(ct);

        var activeMerchants = await _merchantDb.Merchants
            .CountAsync(m => m.Status == MerchantStatus.Active, ct);

        var totalTerminals = await _merchantDb.Terminals.CountAsync(ct);

        var activeTerminals = await _merchantDb.Terminals
            .CountAsync(t => t.Status == TerminalStatus.Active, ct);

        return new MerchantStats
        {
            TotalMerchants = totalMerchants,
            ActiveMerchants = activeMerchants,
            TotalTerminals = totalTerminals,
            ActiveTerminals = activeTerminals
        };
    }
}