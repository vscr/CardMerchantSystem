namespace WorkOrder.Application.DTOs;

public class WorkOrderApprovalDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderItemId { get; set; }
    public int Level { get; set; }
    public string ApproverUsername { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Notes { get; set; }
}

public class ProcessApprovalDto
{
    public Guid ApprovalId { get; set; }
    public bool IsApproved { get; set; }
    public string? Notes { get; set; }
}