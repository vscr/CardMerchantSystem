using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class FraudAlertRepository : IFraudAlertRepository
{
    private readonly FraudDbContext _context;

    public FraudAlertRepository(FraudDbContext context) => _context = context;

    public async Task<FraudAlert?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.FraudAlerts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<FraudAlert?> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct = default)
        => await _context.FraudAlerts.FirstOrDefaultAsync(x => x.TransactionId == transactionId, ct);

    public async Task<List<FraudAlert>> GetByStatusAsync(FraudAlertStatus status, int page = 1, int pageSize = 20, CancellationToken ct = default)
        => await _context.FraudAlerts
            .AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<List<FraudAlert>> GetAssignedToAsync(string operatorUsername, CancellationToken ct = default)
        => await _context.FraudAlerts
            .AsNoTracking()
            .Where(x => x.AssignedTo == operatorUsername && x.Status != FraudAlertStatus.Resolved)
            .OrderByDescending(x => x.TotalScore)
            .ToListAsync(ct);

    public async Task<int> GetCountByStatusAsync(FraudAlertStatus status, CancellationToken ct = default)
        => await _context.FraudAlerts.CountAsync(x => x.Status == status, ct);

    public async Task AddAsync(FraudAlert alert, CancellationToken ct = default)
    {
        await _context.FraudAlerts.AddAsync(alert, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FraudAlert alert, CancellationToken ct = default)
    {
        _context.FraudAlerts.Update(alert);
        await _context.SaveChangesAsync(ct);
    }
}