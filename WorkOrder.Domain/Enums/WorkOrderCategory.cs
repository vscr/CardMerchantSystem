using CardMerchantSystem.Shared.Kernel;

namespace WorkOrder.Domain.Enums;

public class WorkOrderCategory : Enumeration
{
    public static readonly WorkOrderCategory CardOperation = new(1, "CardOperation", "Kart İşlemleri");
    public static readonly WorkOrderCategory LimitChange = new(2, "LimitChange", "Limit İşlemleri");
    public static readonly WorkOrderCategory CustomerInfo = new(3, "CustomerInfo", "Müşteri Bilgileri");
    public static readonly WorkOrderCategory Complaint = new(4, "Complaint", "Şikayet");
    public static readonly WorkOrderCategory Security = new(5, "Security", "Güvenlik");
    public static readonly WorkOrderCategory Other = new(6, "Other", "Diğer");

    private WorkOrderCategory(int id, string name, string displayName)
        : base(id, name, displayName) { }
}