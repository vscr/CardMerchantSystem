using CardMerchantSystem.Shared.Kernel;

namespace WorkOrder.Domain.Enums;

public class ApprovalStatus : Enumeration
{
    public static readonly ApprovalStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly ApprovalStatus Approved = new(2, "Approved", "Onaylandı");
    public static readonly ApprovalStatus Rejected = new(3, "Rejected", "Reddedildi");

    private ApprovalStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool IsFinal => this == Approved || this == Rejected;
}