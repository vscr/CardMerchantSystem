using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using Fee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Repositories;

public class MembershipFeeRepository : IMembershipFeeRepository
{
    private readonly FeeDbContext _context;

    public MembershipFeeRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<MembershipFee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MembershipFees
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MembershipFee?> GetByNameAsync(string feeName, CancellationToken cancellationToken = default)
    {
        return await _context.MembershipFees
            .FirstOrDefaultAsync(x => x.FeeName == feeName, cancellationToken);
    }

    public async Task<IReadOnlyList<MembershipFee>> GetByFeeTypeAsync(FeeType feeType, CancellationToken cancellationToken = default)
    {
        var fees = await _context.MembershipFees
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        return fees.Where(x => x.FeeType.Id == feeType.Id).ToList();
    }

    public async Task<IReadOnlyList<MembershipFee>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MembershipFees
            .Where(x => x.IsActive)
            .OrderBy(x => x.FeeName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MembershipFee fee, CancellationToken cancellationToken = default)
    {
        await _context.MembershipFees.AddAsync(fee, cancellationToken);
    }

    public Task UpdateAsync(MembershipFee fee, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}