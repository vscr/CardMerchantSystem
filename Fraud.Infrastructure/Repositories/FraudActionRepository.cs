using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class FraudActionRepository : IFraudActionRepository
{
    private readonly FraudDbContext _context;

    public FraudActionRepository(FraudDbContext context) => _context = context;

    public async Task<List<FraudAction>> GetByAlertIdAsync(Guid alertId, CancellationToken ct = default)
        => await _context.FraudActions
            .AsNoTracking()
            .Where(x => x.FraudAlertId == alertId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<FraudAction>> GetByCardNoAsync(string maskedCardNo, int days = 90, CancellationToken ct = default)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        return await _context.FraudActions
            .AsNoTracking()
            .Where(x => x.MaskedCardNo == maskedCardNo && x.CreatedAt >= from)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(FraudAction action, CancellationToken ct = default)
    {
        await _context.FraudActions.AddAsync(action, ct);
        await _context.SaveChangesAsync(ct);
    }
}