using CardMerchantSystem.Shared.Kernel;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Domain.Entities;

public class WorkOrderType : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public WorkOrderCategory Category { get; private set; } = null!;
    public int SlaHours { get; private set; }
    public bool RequiresApproval { get; private set; }
    public int ApprovalLevels { get; private set; }
    public bool IsActive { get; private set; }

    private WorkOrderType() { }

    public static Result<WorkOrderType> Create(
        string code, string name, string description,
        WorkOrderCategory category, int slaHours,
        bool requiresApproval, int approvalLevels)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<WorkOrderType>("Kod boş olamaz");

        return new WorkOrderType
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            Category = category,
            SlaHours = slaHours,
            RequiresApproval = requiresApproval,
            ApprovalLevels = requiresApproval ? Math.Max(1, approvalLevels) : 0,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}