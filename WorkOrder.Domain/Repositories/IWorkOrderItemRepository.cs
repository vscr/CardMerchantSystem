using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Domain.Repositories;

public interface IWorkOrderItemRepository
{
    Task<WorkOrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkOrderItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkOrderItem?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderItem>> GetByStatusAsync(WorkOrderStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderItem>> GetByAssignedToAsync(string assignedTo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderItem>> GetOpenOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderItem>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrderItem>> GetPendingApprovalAsync(string approverUsername, CancellationToken cancellationToken = default);
    Task AddAsync(WorkOrderItem item, CancellationToken cancellationToken = default);
    void Update(WorkOrderItem item);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}