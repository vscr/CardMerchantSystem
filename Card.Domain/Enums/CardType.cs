using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Enums;

/// <summary>
/// Kart tipleri.
/// Her kart tipi farklı limit ve özellikler taşır.
/// </summary>
public class CardType : Enumeration
{
    public static readonly CardType Debit = new(1, nameof(Debit), "Banka Kartı",
        defaultDailyLimit: 10000, defaultMonthlyLimit: 50000);

    public static readonly CardType Credit = new(2, nameof(Credit), "Kredi Kartı",
        defaultDailyLimit: 25000, defaultMonthlyLimit: 100000);

    public static readonly CardType Prepaid = new(3, nameof(Prepaid), "Ön Ödemeli Kart",
        defaultDailyLimit: 5000, defaultMonthlyLimit: 20000);

    public static readonly CardType Virtual = new(4, nameof(Virtual), "Sanal Kart",
        defaultDailyLimit: 5000, defaultMonthlyLimit: 15000);

    public static readonly CardType Commercial = new(5, nameof(Commercial), "Ticari Kart",
        defaultDailyLimit: 100000, defaultMonthlyLimit: 500000);

    public decimal DefaultDailyLimit { get; }
    public decimal DefaultMonthlyLimit { get; }

    private CardType(int id, string name, string displayName,
        decimal defaultDailyLimit, decimal defaultMonthlyLimit)
        : base(id, name, displayName)
    {
        DefaultDailyLimit = defaultDailyLimit;
        DefaultMonthlyLimit = defaultMonthlyLimit;
    }

    /// <summary>
    /// Fiziksel kart mı?
    /// </summary>
    public bool IsPhysical => this != Virtual;

    /// <summary>
    /// Kredi özelliği var mı?
    /// </summary>
    public bool HasCreditFeature => this == Credit || this == Commercial;
}