using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Enums;

/// <summary>
/// Muhasebe Fişi Durumları
/// </summary>
public class JournalEntryStatus : Enumeration
{
    public static readonly JournalEntryStatus Draft = new(1, nameof(Draft), "Taslak");
    public static readonly JournalEntryStatus Posted = new(2, nameof(Posted), "Onaylandı");
    public static readonly JournalEntryStatus Reversed = new(3, nameof(Reversed), "İptal Edildi");

    private JournalEntryStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool IsEditable => this == Draft;
    public bool IsPosted => this == Posted;
}