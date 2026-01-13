using Statement.Domain.Entities;
using Statement.Domain.Repositories;
using Statement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Statement.Infrastructure.Repositories;

public class StatementPeriodConfigRepository : IStatementPeriodConfigRepository
{
    private readonly StatementDbContext _context;

    public StatementPeriodConfigRepository(StatementDbContext context)
    {
        _context = context;
    }

    public async Task<StatementPeriodConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StatementPeriodConfigs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<StatementPeriodConfig?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.StatementPeriodConfigs
            .FirstOrDefaultAsync(x => x.CardNumber == cardNumber && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<StatementPeriodConfig>> GetByStatementDayAsync(int statementDay, CancellationToken cancellationToken = default)
    {
        return await _context.StatementPeriodConfigs
            .Where(x => x.StatementDay == statementDay && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StatementPeriodConfig>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StatementPeriodConfigs
            .Where(x => x.IsActive)
            .OrderBy(x => x.CardNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StatementPeriodConfig config, CancellationToken cancellationToken = default)
    {
        await _context.StatementPeriodConfigs.AddAsync(config, cancellationToken);
    }

    public Task UpdateAsync(StatementPeriodConfig config, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}