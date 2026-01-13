using Accounting.Domain.Entities;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class AccountBalanceRepository : IAccountBalanceRepository
{
    private readonly AccountingDbContext _context;

    public AccountBalanceRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<AccountBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccountBalances
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AccountBalance?> GetByAccountAndPeriodAsync(Guid accountId, string periodCode, CancellationToken cancellationToken = default)
    {
        return await _context.AccountBalances
            .FirstOrDefaultAsync(x => x.AccountId == accountId && x.PeriodCode == periodCode, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountBalance>> GetByPeriodAsync(string periodCode, CancellationToken cancellationToken = default)
    {
        return await _context.AccountBalances
            .Where(x => x.PeriodCode == periodCode)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccountBalance>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.AccountBalances
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.PeriodCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccountBalance>> GetNonZeroBalancesByPeriodAsync(string periodCode, CancellationToken cancellationToken = default)
    {
        return await _context.AccountBalances
            .Where(x => x.PeriodCode == periodCode &&
                       (x.ClosingDebit != 0 || x.ClosingCredit != 0))
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AccountBalance balance, CancellationToken cancellationToken = default)
    {
        await _context.AccountBalances.AddAsync(balance, cancellationToken);
    }

    public Task UpdateAsync(AccountBalance balance, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}