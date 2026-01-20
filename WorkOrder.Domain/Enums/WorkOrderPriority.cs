using CardMerchantSystem.Shared.Kernel;

namespace WorkOrder.Domain.Enums;

public class WorkOrderPriority : Enumeration
{
    public static readonly WorkOrderPriority Low = new(1, "Low", "Düşük");
    public static readonly WorkOrderPriority Normal = new(2, "Normal", "Normal");
    public static readonly WorkOrderPriority High = new(3, "High", "Yüksek");
    public static readonly WorkOrderPriority Urgent = new(4, "Urgent", "Acil");

    private WorkOrderPriority(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public int SlaMultiplier => this.Id switch
    {
        4 => 25,   // %25 süre
        3 => 50,   // %50 süre
        2 => 100,  // Normal süre
        1 => 150,  // %150 süre
        _ => 100
    };
}