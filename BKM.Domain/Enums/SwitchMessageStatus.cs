using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Enums;

/// <summary>
/// Switch mesaj durumları
/// </summary>
public class SwitchMessageStatus : Enumeration
{
    public static readonly SwitchMessageStatus Received = new(1, nameof(Received), "Alındı");
    public static readonly SwitchMessageStatus Validated = new(2, nameof(Validated), "Doğrulandı");
    public static readonly SwitchMessageStatus Routed = new(3, nameof(Routed), "Yönlendirildi");
    public static readonly SwitchMessageStatus Processed = new(4, nameof(Processed), "İşlendi");
    public static readonly SwitchMessageStatus Responded = new(5, nameof(Responded), "Yanıtlandı");
    public static readonly SwitchMessageStatus Failed = new(6, nameof(Failed), "Başarısız");
    public static readonly SwitchMessageStatus Timeout = new(7, nameof(Timeout), "Zaman Aşımı");

    private SwitchMessageStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Final durum mu?
    /// </summary>
    public bool IsFinal => this == Responded || this == Failed || this == Timeout;
}