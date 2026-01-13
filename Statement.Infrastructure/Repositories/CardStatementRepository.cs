using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Statement.Domain.Repositories;
using Statement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Statement.Infrastructure.Repositories;

public class CardStatementRepository : ICardStatementRepository
{
    private readonly StatementDbContext _context;

    public CardStatementRepository(StatementDbContext context)
    {
        _context = context;
    }

    public async Task<CardStatement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardStatement?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardStatement?> GetByStatementNumberAsync(string statementNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.StatementNumber == statementNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<CardStatement>> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .Where(x => x.CardNumber == cardNumber)
            .OrderByDescending(x => x.PeriodEndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<CardStatement?> GetLatestByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .Where(x => x.CardNumber == cardNumber)
            .OrderByDescending(x => x.PeriodEndDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardStatement>> GetByStatusAsync(StatementStatus status, CancellationToken cancellationToken = default)
    {
        var statements = await _context.CardStatements
            .OrderByDescending(x => x.PeriodEndDate)
            .ToListAsync(cancellationToken);

        return statements.Where(x => x.Status.Id == status.Id).ToList();
    }

    public async Task<IReadOnlyList<CardStatement>> GetOverdueStatementsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var statements = await _context.CardStatements
            .Where(x => x.DueDate < today)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);

        return statements.Where(x => x.Status.RequiresPayment).ToList();
    }

    public async Task<IReadOnlyList<CardStatement>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.CardStatements
            .Where(x => x.DueDate >= startDate && x.DueDate <= endDate)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CardStatement>> GetPendingForNotificationAsync(CancellationToken cancellationToken = default)
    {
        var statements = await _context.CardStatements
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return statements.Where(x => x.Status == StatementStatus.Generated).ToList();
    }

    public async Task AddAsync(CardStatement statement, CancellationToken cancellationToken = default)
    {
        await _context.CardStatements.AddAsync(statement, cancellationToken);
    }

    public Task UpdateAsync(CardStatement statement, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}