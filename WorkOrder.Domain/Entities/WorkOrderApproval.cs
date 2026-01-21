using CardMerchantSystem.Shared.Kernel;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Domain.Entities;

public class WorkOrderApproval : Entity
{
    public Guid WorkOrderItemId { get; private set; }
    public int Level { get; private set; }
    public string ApproverUsername { get; private set; } = null!;
    public ApprovalStatus Status { get; private set; } = null!;
    public DateTime RequestedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Notes { get; private set; }

    private WorkOrderApproval() { }

    public static WorkOrderApproval Create(Guid workOrderItemId, int level, string approverUsername)
    {
        return new WorkOrderApproval
        {
            WorkOrderItemId = workOrderItemId,
            Level = level,
            ApproverUsername = approverUsername,
            Status = ApprovalStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }

    public void Approve(string? notes)
    {
        Status = ApprovalStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        Notes = notes;
    }

    public void Reject(string? notes)
    {
        Status = ApprovalStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        Notes = notes;
    }
}