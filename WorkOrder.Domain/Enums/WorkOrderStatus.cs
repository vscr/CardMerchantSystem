using CardMerchantSystem.Shared.Kernel;

namespace WorkOrder.Domain.Enums;

public class WorkOrderStatus : Enumeration
{
    public static readonly WorkOrderStatus Open = new(1, "Open", "Açık");
    public static readonly WorkOrderStatus InProgress = new(2, "InProgress", "İşlemde");
    public static readonly WorkOrderStatus PendingApproval = new(3, "PendingApproval", "Onay Bekliyor");
    public static readonly WorkOrderStatus OnHold = new(4, "OnHold", "Beklemede");
    public static readonly WorkOrderStatus Completed = new(5, "Completed", "Tamamlandı");
    public static readonly WorkOrderStatus Cancelled = new(6, "Cancelled", "İptal");
    public static readonly WorkOrderStatus Rejected = new(7, "Rejected", "Reddedildi");

    private WorkOrderStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool IsOpen => this == Open || this == InProgress || this == PendingApproval || this == OnHold;
    public bool IsClosed => this == Completed || this == Cancelled || this == Rejected;
    public bool CanCancel => this == Open || this == InProgress || this == OnHold;
}