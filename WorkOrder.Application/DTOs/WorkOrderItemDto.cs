namespace WorkOrder.Application.DTOs;

public class WorkOrderItemDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public Guid WorkOrderTypeId { get; set; }
    public string TypeName { get; set; } = null!;
    public string TypeCode { get; set; } = null!;
    public Guid? CardId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string PriorityDisplayName { get; set; } = null!;
    public string? AssignedTo { get; set; }
    public string? AssignedTeam { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }
    public string? Resolution { get; set; }
    public int NoteCount { get; set; }
    public int ApprovalCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WorkOrderItemWithDetailsDto : WorkOrderItemDto
{
    public List<WorkOrderNoteDto> Notes { get; set; } = new();
    public List<WorkOrderApprovalDto> Approvals { get; set; } = new();
}

public class CreateWorkOrderItemDto
{
    public Guid WorkOrderTypeId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int PriorityId { get; set; }
    public Guid? CardId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}

public class AssignWorkOrderDto
{
    public string AssignedTo { get; set; } = null!;
    public string? AssignedTeam { get; set; }
}