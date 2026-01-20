using CardMerchantSystem.Shared.Kernel;

namespace WorkOrder.Domain.Entities;

public class WorkOrderNote : Entity
{
    public Guid WorkOrderItemId { get; private set; }
    public string Content { get; private set; } = null!;
    public bool IsInternal { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;

    private WorkOrderNote() { }

    public static WorkOrderNote Create(Guid workOrderItemId, string content, string createdBy, bool isInternal)
    {
        return new WorkOrderNote
        {
            WorkOrderItemId = workOrderItemId,
            Content = content,
            CreatedBy = createdBy,
            IsInternal = isInternal,
            CreatedAt = DateTime.UtcNow
        };
    }
}