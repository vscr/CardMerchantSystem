using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Enums;

/// <summary>
/// İşlem tipleri
/// </summary>
public class TransactionType : Enumeration
{
    public static readonly TransactionType Sale = new(1, nameof(Sale), "Satış");
    public static readonly TransactionType Refund = new(2, nameof(Refund), "İade");
    public static readonly TransactionType Cancel = new(3, nameof(Cancel), "İptal");
    public static readonly TransactionType PreAuth = new(4, nameof(PreAuth), "Ön Provizyon");
    public static readonly TransactionType PostAuth = new(5, nameof(PostAuth), "Ön Provizyon Kapama");
    public static readonly TransactionType CashAdvance = new(6, nameof(CashAdvance), "Nakit Avans");

    private TransactionType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Limit düşen işlem mi?
    /// </summary>
    public bool DecreasesLimit => this == Sale || this == PreAuth || this == CashAdvance;

    /// <summary>
    /// Limit artıran işlem mi?
    /// </summary>
    public bool IncreasesLimit => this == Refund || this == Cancel;
}