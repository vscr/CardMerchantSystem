using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Enums;

/// <summary>
/// Muhasebe Dönem Durumları
/// </summary>
public class PeriodStatus : Enumeration
{
    public static readonly PeriodStatus Open = new(1, nameof(Open), "Açık");
    public static readonly PeriodStatus Closing = new(2, nameof(Closing), "Kapanıyor");
    public static readonly PeriodStatus Closed = new(3, nameof(Closed), "Kapalı");
    public static readonly PeriodStatus Locked = new(4, nameof(Locked), "Kilitli");

    private PeriodStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool AllowsPosting => this == Open;
    public bool IsClosed => this == Closed || this == Locked;
}