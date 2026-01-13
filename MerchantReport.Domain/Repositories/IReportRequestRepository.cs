using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;

namespace MerchantReport.Domain.Repositories;

/// <summary>
/// Rapor Talebi Repository Interface
/// </summary>
public interface IReportRequestRepository
{
    Task<ReportRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReportRequest?> GetByRequestNumberAsync(string requestNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportRequest>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportRequest>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportRequest>> GetFailedRequestsForRetryAsync(int maxRetryCount, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(ReportRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReportRequest request, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}