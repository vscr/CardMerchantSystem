namespace WorkOrder.Application.DTOs;

public class WorkOrderTypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string CategoryDisplayName { get; set; } = null!;
    public int SlaHours { get; set; }
    public bool RequiresApproval { get; set; }
    public int ApprovalLevels { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWorkOrderTypeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CategoryId { get; set; }
    public int SlaHours { get; set; }
    public bool RequiresApproval { get; set; }
    public int ApprovalLevels { get; set; }
}