using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Repositories;
using MerchantReport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MerchantReport.Infrastructure.Repositories;

public class MerchantStatementRepository : IMerchantStatementRepository
{
    private readonly MerchantReportDbContext _context;

    public MerchantStatementRepository(MerchantReportDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantStatement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantStatement?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MerchantStatement?> GetByStatementNumberAsync(string statementNumber, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.StatementNumber == statementNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantStatement>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.PeriodEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<MerchantStatement?> GetLatestByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.PeriodEnd)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MerchantStatement>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantStatements
            .Where(x => x.PeriodStart >= periodStart && x.PeriodEnd <= periodEnd)
            .OrderByDescending(x => x.PeriodEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MerchantStatement statement, CancellationToken cancellationToken = default)
    {
        await _context.MerchantStatements.AddAsync(statement, cancellationToken);
    }

    public Task UpdateAsync(MerchantStatement statement, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}