using CardMerchantSystem.Shared.Kernel;

namespace EarlyBlockResolution.Domain.Enums;

/// <summary>
/// Uyarı önem dereceleri
/// </summary>
public class AlertSeverity : Enumeration
{
    public static readonly AlertSeverity Low = new(1, "Low", "Düşük");
    public static readonly AlertSeverity Medium = new(2, "Medium", "Orta");
    public static readonly AlertSeverity High = new(3, "High", "Yüksek");
    public static readonly AlertSeverity Critical = new(4, "Critical", "Kritik");

    private AlertSeverity(int id, string name, string displayName)
        : base(id, name, displayName) { }

    public bool RequiresImmediateBlock => this == High || this == Critical;
    public int ResponseTimeMinutes => this.Id switch
    {
        1 => 1440,  // 24 saat
        2 => 240,   // 4 saat
        3 => 60,    // 1 saat
        4 => 15,    // 15 dakika
        _ => 60
    };
}