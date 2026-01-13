using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class AccountingPeriodRepository : IAccountingPeriodRepository
{
    private readonly AccountingDbContext _context;

    public AccountingPeriodRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<AccountingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AccountingPeriod?> GetByCodeAsync(string periodCode, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .FirstOrDefaultAsync(x => x.PeriodCode == periodCode, cancellationToken);
    }

    public async Task<AccountingPeriod?> GetCurrentPeriodAsync(CancellationToken cancellationToken = default)
    {
        var periods = await _context.AccountingPeriods
            .OrderByDescending(x => x.PeriodCode)
            .ToListAsync(cancellationToken);

        return periods.FirstOrDefault(x => x.Status == PeriodStatus.Open);
    }

    public async Task<IReadOnlyList<AccountingPeriod>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .Where(x => x.Year == year)
            .OrderBy(x => x.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccountingPeriod>> GetByStatusAsync(PeriodStatus status, CancellationToken cancellationToken = default)
    {
        var periods = await _context.AccountingPeriods
            .OrderByDescending(x => x.PeriodCode)
            .ToListAsync(cancellationToken);

        return periods.Where(x => x.Status.Id == status.Id).ToList();
    }

    public async Task<IReadOnlyList<AccountingPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .OrderByDescending(x => x.PeriodCode)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AccountingPeriod period, CancellationToken cancellationToken = default)
    {
        await _context.AccountingPeriods.AddAsync(period, cancellationToken);
    }

    public Task UpdateAsync(AccountingPeriod period, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}