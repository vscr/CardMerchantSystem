using Microsoft.EntityFrameworkCore;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Repositories;
using WorkOrder.Infrastructure.Persistence;

namespace WorkOrder.Infrastructure.Repositories;

public class WorkOrderTypeRepository : IWorkOrderTypeRepository
{
    private readonly WorkOrderDbContext _context;

    public WorkOrderTypeRepository(WorkOrderDbContext context) => _context = context;

    public async Task<WorkOrderType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.WorkOrderTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<WorkOrderType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.WorkOrderTypes.FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);

    public async Task<IReadOnlyList<WorkOrderType>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.WorkOrderTypes.OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkOrderType>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _context.WorkOrderTypes.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(WorkOrderType type, CancellationToken cancellationToken = default)
        => await _context.WorkOrderTypes.AddAsync(type, cancellationToken);

    public void Update(WorkOrderType type) => _context.WorkOrderTypes.Update(type);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}