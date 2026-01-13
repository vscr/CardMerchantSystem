using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;
using MerchantReport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantReport.Infrastructure.Repositories;

public class ReportRequestRepository : IReportRequestRepository
{
    private readonly MerchantReportDbContext _context;

    public ReportRequestRepository(MerchantReportDbContext context)
    {
        _context = context;
    }

    public async Task<ReportRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReportRequest?> GetByRequestNumberAsync(string requestNumber, CancellationToken cancellationToken = default)
    {
        return await _context.ReportRequests
            .FirstOrDefaultAsync(x => x.RequestNumber == requestNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportRequest>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.ReportRequests
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportRequest>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        var requests = await _context.ReportRequests
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Where(x => x.Status.Id == status.Id).ToList();
    }

    public async Task<IReadOnlyList<ReportRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _context.ReportRequests
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Where(x => x.Status == ReportStatus.Pending).ToList();
    }

    public async Task<IReadOnlyList<ReportRequest>> GetFailedRequestsForRetryAsync(int maxRetryCount, CancellationToken cancellationToken = default)
    {
        var requests = await _context.ReportRequests
            .Where(x => x.RetryCount < maxRetryCount)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Where(x => x.Status == ReportStatus.Failed).ToList();
    }

    public async Task<IReadOnlyList<ReportRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.ReportRequests
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        await _context.ReportRequests.AddAsync(request, cancellationToken);
    }

    public Task UpdateAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}