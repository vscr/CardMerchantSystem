using Microsoft.EntityFrameworkCore;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;
using WorkOrder.Domain.Repositories;
using WorkOrder.Infrastructure.Persistence;

namespace WorkOrder.Infrastructure.Repositories;

public class WorkOrderItemRepository : IWorkOrderItemRepository
{
    private readonly WorkOrderDbContext _context;

    public WorkOrderItemRepository(WorkOrderDbContext context) => _context = context;

    public async Task<WorkOrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<WorkOrderItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Type)
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<WorkOrderItem?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, cancellationToken);

    public async Task<IReadOnlyList<WorkOrderItem>> GetByStatusAsync(WorkOrderStatus status, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkOrderItem>> GetByAssignedToAsync(string assignedTo, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => x.AssignedTo == assignedTo && x.Status.Id < 5)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkOrderItem>> GetOpenOrdersAsync(CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => x.Status == WorkOrderStatus.Open ||
                        x.Status == WorkOrderStatus.InProgress ||
                        x.Status == WorkOrderStatus.PendingApproval ||
                        x.Status == WorkOrderStatus.OnHold)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.DueDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkOrderItem>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => x.DueDate < now &&
                        (x.Status == WorkOrderStatus.Open ||
                         x.Status == WorkOrderStatus.InProgress ||
                         x.Status == WorkOrderStatus.PendingApproval ||
                         x.Status == WorkOrderStatus.OnHold))
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkOrderItem>> GetCompletedOrdersAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => 
                        x.Status == WorkOrderStatus.Completed )
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkOrderItem>> GetPendingApprovalAsync(string approverUsername, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems
            .Include(x => x.Notes)
            .Include(x => x.Approvals)
            .Where(x => x.Status == WorkOrderStatus.PendingApproval &&
                        x.Approvals.Any(a => a.ApproverUsername == approverUsername && a.Status == ApprovalStatus.Pending))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(WorkOrderItem item, CancellationToken cancellationToken = default)
        => await _context.WorkOrderItems.AddAsync(item, cancellationToken);

    public void Update(WorkOrderItem item) => _context.WorkOrderItems.Update(item);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}