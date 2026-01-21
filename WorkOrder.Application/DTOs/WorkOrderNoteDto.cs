namespace WorkOrder.Application.DTOs;

public class WorkOrderNoteDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderItemId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class AddNoteDto
{
    public string Content { get; set; } = null!;
    public bool IsInternal { get; set; }
}