using WorkOrder.Domain.Entities;

namespace WorkOrder.Domain.Repositories;

public interface IWorkOrderTypeRepository
{
    Task<WorkOrderType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkOrderType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(WorkOrderType type, CancellationToken cancellationToken = default);
    void Update(WorkOrderType type);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}