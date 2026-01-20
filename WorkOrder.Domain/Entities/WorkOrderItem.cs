using CardMerchantSystem.Shared.Kernel;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Domain.Entities;

public class WorkOrderItem : AggregateRoot
{
    public string OrderNumber { get; private set; } = null!;
    public Guid WorkOrderTypeId { get; private set; }
    public WorkOrderType Type { get; private set; } = null!;

    // İlişkili kayıtlar
    public Guid? CardId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }

    // Detaylar
    public string Subject { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public WorkOrderStatus Status { get; private set; } = null!;
    public WorkOrderPriority Priority { get; private set; } = null!;

    // Atama
    public string? AssignedTo { get; private set; }
    public string? AssignedTeam { get; private set; }

    // SLA
    public DateTime DueDate { get; private set; }
    public bool IsOverdue => Status.IsOpen && DateTime.UtcNow > DueDate;

    // Tarihler
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletedBy { get; private set; }
    public string? Resolution { get; private set; }

    // Alt koleksiyonlar
    private readonly List<WorkOrderNote> _notes = new();
    public IReadOnlyCollection<WorkOrderNote> Notes => _notes.AsReadOnly();

    private readonly List<WorkOrderApproval> _approvals = new();
    public IReadOnlyCollection<WorkOrderApproval> Approvals => _approvals.AsReadOnly();

    private WorkOrderItem() { }

    public static Result<WorkOrderItem> Create(
        Guid workOrderTypeId, int slaHours, bool requiresApproval,
        string subject, string description, WorkOrderPriority priority,
        Guid? cardId, Guid? customerId, string? customerName, string? customerPhone)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return Result.Failure<WorkOrderItem>("Konu boş olamaz");

        var adjustedSlaHours = slaHours * priority.SlaMultiplier / 100;

        return new WorkOrderItem
        {
            OrderNumber = $"WO{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}",
            WorkOrderTypeId = workOrderTypeId,
            CardId = cardId,
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            Subject = subject,
            Description = description,
            Status = requiresApproval ? WorkOrderStatus.PendingApproval : WorkOrderStatus.Open,
            Priority = priority,
            DueDate = DateTime.UtcNow.AddHours(adjustedSlaHours)
        };
    }

    public Result Assign(string assignedTo, string? team, string operatorUsername)
    {
        if (Status.IsClosed)
            return Result.Failure("Kapalı iş emri atanamaz");

        AssignedTo = assignedTo;
        AssignedTeam = team;
        if (Status == WorkOrderStatus.Open)
        {
            Status = WorkOrderStatus.InProgress;
            StartedAt = DateTime.UtcNow;
        }
        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    public Result Start(string operatorUsername)
    {
        if (Status != WorkOrderStatus.Open && Status != WorkOrderStatus.OnHold)
            return Result.Failure($"Bu durumda başlatılamaz: {Status.DisplayName}");

        Status = WorkOrderStatus.InProgress;
        StartedAt ??= DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    public Result PutOnHold(string reason, string operatorUsername)
    {
        if (!Status.IsOpen)
            return Result.Failure("Sadece açık iş emirleri bekletmeye alınabilir");

        Status = WorkOrderStatus.OnHold;
        AddNote(reason, operatorUsername, true);
        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    public Result Complete(string resolution, string completedBy)
    {
        if (Status.IsClosed)
            return Result.Failure("İş emri zaten kapalı");

        Status = WorkOrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = completedBy;
        Resolution = resolution;
        MarkAsUpdated(completedBy);
        return Result.Success();
    }

    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez: {Status.DisplayName}");

        Status = WorkOrderStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        Resolution = $"İptal: {reason}";
        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    public Result Reject(string reason, string operatorUsername)
    {
        Status = WorkOrderStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
        Resolution = $"Red: {reason}";
        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    public void AddNote(string content, string createdBy, bool isInternal = false)
    {
        var note = WorkOrderNote.Create(Id, content, createdBy, isInternal);
        _notes.Add(note);
    }

    public Result<WorkOrderApproval> RequestApproval(int level, string approverUsername)
    {
        var approval = WorkOrderApproval.Create(Id, level, approverUsername);
        _approvals.Add(approval);
        Status = WorkOrderStatus.PendingApproval;
        return approval;
    }

    public Result ProcessApproval(Guid approvalId, bool isApproved, string? notes, string operatorUsername)
    {
        var approval = _approvals.FirstOrDefault(a => a.Id == approvalId);
        if (approval is null)
            return Result.Failure("Onay bulunamadı");

        if (isApproved)
            approval.Approve(notes);
        else
            approval.Reject(notes);

        if (!isApproved)
        {
            Status = WorkOrderStatus.Rejected;
            Resolution = $"Onay reddedildi: {notes}";
        }
        else if (_approvals.All(a => a.Status == ApprovalStatus.Approved))
        {
            Status = WorkOrderStatus.Open;
        }

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }
}