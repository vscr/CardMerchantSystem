using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Enums;

/// <summary>
/// Mutabakat durumları
/// </summary>
public class ReconciliationStatus : Enumeration
{
    public static readonly ReconciliationStatus Pending = new(1, "Pending", "Beklemede");
    public static readonly ReconciliationStatus Matched = new(2, "Matched", "Eşleşti");
    public static readonly ReconciliationStatus Mismatched = new(3, "Mismatched", "Uyuşmazlık");
    public static readonly ReconciliationStatus PartiallyMatched = new(4, "PartiallyMatched", "Kısmen Eşleşti");
    public static readonly ReconciliationStatus Resolved = new(5, "Resolved", "Çözüldü");
    public static readonly ReconciliationStatus Disputed = new(6, "Disputed", "İtiraz Edildi");

    private ReconciliationStatus(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool NeedsAttention => this == Mismatched || this == PartiallyMatched || this == Disputed;
    public bool IsResolved => this == Matched || this == Resolved;
}