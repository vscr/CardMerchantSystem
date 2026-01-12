using Fee.Domain.Entities;
using Fee.Domain.Enums;

namespace Fee.Domain.Repositories;

/// <summary>
/// Aidat Repository Interface
/// </summary>
public interface IMembershipFeeRepository
{
    Task<MembershipFee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MembershipFee?> GetByNameAsync(string feeName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MembershipFee>> GetByFeeTypeAsync(FeeType feeType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MembershipFee>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MembershipFee fee, CancellationToken cancellationToken = default);
    Task UpdateAsync(MembershipFee fee, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}