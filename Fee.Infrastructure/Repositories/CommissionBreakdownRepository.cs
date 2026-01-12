using Fee.Domain.Entities;
using Fee.Domain.Repositories;
using Fee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Repositories;

public class CommissionBreakdownRepository : ICommissionBreakdownRepository
{
    private readonly FeeDbContext _context;

    public CommissionBreakdownRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<CommissionBreakdown?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionBreakdowns
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CommissionBreakdown?> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionBreakdowns
            .FirstOrDefaultAsync(x => x.TransactionId == transactionId, cancellationToken);
    }

    public async Task<IReadOnlyList<CommissionBreakdown>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionBreakdowns
            .Where(x => x.MerchantId == merchantId)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CommissionBreakdown>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionBreakdowns
            .Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CommissionBreakdown>> GetByMerchantAndDateRangeAsync(string merchantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.CommissionBreakdowns
            .Where(x => x.MerchantId == merchantId &&
                        x.TransactionDate >= startDate &&
                        x.TransactionDate <= endDate)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MerchantCommissionSummary> GetMerchantSummaryAsync(string merchantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var breakdowns = await _context.CommissionBreakdowns
            .Where(x => x.MerchantId == merchantId &&
                        x.TransactionDate >= startDate &&
                        x.TransactionDate <= endDate)
            .ToListAsync(cancellationToken);

        if (!breakdowns.Any())
        {
            return new MerchantCommissionSummary
            {
                MerchantId = merchantId,
                TransactionCount = 0,
                TotalTransactionAmount = 0,
                TotalCommission = 0,
                TotalBankShare = 0,
                TotalInterchangeFee = 0,
                TotalBKMFee = 0,
                TotalMerchantNet = 0,
                AverageCommissionRate = 0
            };
        }

        return new MerchantCommissionSummary
        {
            MerchantId = merchantId,
            TransactionCount = breakdowns.Count,
            TotalTransactionAmount = breakdowns.Sum(x => x.TransactionAmount),
            TotalCommission = breakdowns.Sum(x => x.TotalCommission),
            TotalBankShare = breakdowns.Sum(x => x.BankShare),
            TotalInterchangeFee = breakdowns.Sum(x => x.InterchangeFee),
            TotalBKMFee = breakdowns.Sum(x => x.BKMFee),
            TotalMerchantNet = breakdowns.Sum(x => x.MerchantNetAmount),
            AverageCommissionRate = breakdowns.Average(x => x.CommissionRate)
        };
    }

    public async Task AddAsync(CommissionBreakdown breakdown, CancellationToken cancellationToken = default)
    {
        await _context.CommissionBreakdowns.AddAsync(breakdown, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}