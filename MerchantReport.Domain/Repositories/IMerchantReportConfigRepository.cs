using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;

namespace MerchantReport.Domain.Repositories;

/// <summary>
/// Üye İşyeri Rapor Ayarları Repository Interface
/// </summary>
public interface IMerchantReportConfigRepository
{
    Task<MerchantReportConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReportConfig>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReportConfig>> GetActiveConfigsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReportConfig>> GetDueConfigsAsync(DateTime asOfTime, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MerchantReportConfig>> GetByReportTypeAsync(ReportType reportType, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantReportConfig config, CancellationToken cancellationToken = default);
    Task UpdateAsync(MerchantReportConfig config, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}