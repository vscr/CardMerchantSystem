using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class ChartOfAccountRepository : IChartOfAccountRepository
{
    private readonly AccountingDbContext _context;

    public ChartOfAccountRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<ChartOfAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ChartOfAccount?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .FirstOrDefaultAsync(x => x.AccountCode == accountCode, cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(x => x.IsActive)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetByTypeAsync(AccountType accountType, CancellationToken cancellationToken = default)
    {
        var accounts = await _context.ChartOfAccounts
            .Where(x => x.IsActive)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);

        return accounts.Where(x => x.AccountType.Id == accountType.Id).ToList();
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(x => x.ParentAccountId == parentId && x.IsActive)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetPostableAccountsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(x => x.IsActive && x.IsPostable)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ChartOfAccount account, CancellationToken cancellationToken = default)
    {
        await _context.ChartOfAccounts.AddAsync(account, cancellationToken);
    }

    public Task UpdateAsync(ChartOfAccount account, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}