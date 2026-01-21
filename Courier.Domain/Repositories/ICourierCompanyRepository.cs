using Courier.Domain.Entities;

namespace Courier.Domain.Repositories;

public interface ICourierCompanyRepository
{
    Task<CourierCompany?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CourierCompany?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourierCompany>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourierCompany>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CourierCompany company, CancellationToken cancellationToken = default);
    void Update(CourierCompany company);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}