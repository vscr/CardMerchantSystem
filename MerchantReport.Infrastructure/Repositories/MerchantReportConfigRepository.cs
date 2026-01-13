using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;
using MerchantReport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantReport.Infrastructure.Repositories;

public class MerchantReportConfigRepository : IMerchantReportConfigRepository
{
    private readonly MerchantReportDbContext _context;

    public MerchantReportConfigRepository(MerchantReportDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantReportConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantReportConfigs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReportConfig>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantReportConfigs
            .Where(x => x.MerchantId == merchantId)
            .OrderBy(x => x.ReportType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReportConfig>> GetActiveConfigsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MerchantReportConfigs
            .Where(x => x.IsActive)
            .OrderBy(x => x.MerchantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReportConfig>> GetDueConfigsAsync(DateTime asOfTime, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantReportConfigs
            .Where(x => x.IsActive && x.NextRunTime != null && x.NextRunTime <= asOfTime)
            .OrderBy(x => x.NextRunTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantReportConfig>> GetByReportTypeAsync(ReportType reportType, CancellationToken cancellationToken = default)
    {
        var configs = await _context.MerchantReportConfigs
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        return configs.Where(x => x.ReportType.Id == reportType.Id).ToList();
    }

    public async Task AddAsync(MerchantReportConfig config, CancellationToken cancellationToken = default)
    {
        await _context.MerchantReportConfigs.AddAsync(config, cancellationToken);
    }

    public Task UpdateAsync(MerchantReportConfig config, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}