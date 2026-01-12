using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using Fee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Repositories;

public class FeeAccrualRepository : IFeeAccrualRepository
{
    private readonly FeeDbContext _context;

    public FeeAccrualRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<FeeAccrual?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FeeAccruals
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<FeeAccrual?> GetByAccrualNumberAsync(string accrualNumber, CancellationToken cancellationToken = default)
    {
        return await _context.FeeAccruals
            .FirstOrDefaultAsync(x => x.AccrualNumber == accrualNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<FeeAccrual>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.FeeAccruals
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.AccrualDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FeeAccrual>> GetByStatusAsync(AccrualStatus status, CancellationToken cancellationToken = default)
    {
        var accruals = await _context.FeeAccruals
            .OrderByDescending(x => x.AccrualDate)
            .ToListAsync(cancellationToken);

        return accruals.Where(x => x.Status.Id == status.Id).ToList();
    }

    public async Task<IReadOnlyList<FeeAccrual>> GetPendingByDueDateAsync(DateTime dueDate, CancellationToken cancellationToken = default)
    {
        var accruals = await _context.FeeAccruals
            .Where(x => x.DueDate <= dueDate)
            .ToListAsync(cancellationToken);

        return accruals.Where(x => x.Status.RequiresPayment).ToList();
    }

    public async Task<IReadOnlyList<FeeAccrual>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var accruals = await _context.FeeAccruals
            .Where(x => x.DueDate < today)
            .ToListAsync(cancellationToken);

        return accruals.Where(x => x.Status.RequiresPayment).ToList();
    }

    public async Task<IReadOnlyList<FeeAccrual>> GetByPeriodAsync(string periodStart, string periodEnd, CancellationToken cancellationToken = default)
    {
        return await _context.FeeAccruals
            .Where(x => x.AccrualPeriodStart.CompareTo(periodStart) >= 0 &&
                        x.AccrualPeriodEnd.CompareTo(periodEnd) <= 0)
            .OrderByDescending(x => x.AccrualDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalUnpaidByMerchantAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        var accruals = await _context.FeeAccruals
            .Where(x => x.MerchantId == merchantId)
            .ToListAsync(cancellationToken);

        return accruals
            .Where(x => x.Status.RequiresPayment)
            .Sum(x => x.RemainingAmount);
    }

    public async Task AddAsync(FeeAccrual accrual, CancellationToken cancellationToken = default)
    {
        await _context.FeeAccruals.AddAsync(accrual, cancellationToken);
    }

    public Task UpdateAsync(FeeAccrual accrual, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}