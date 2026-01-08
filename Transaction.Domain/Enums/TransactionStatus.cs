using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Enums;

/// <summary>
/// İşlem durumları
/// </summary>
public class TransactionStatus : Enumeration
{
    public static readonly TransactionStatus Pending = new(1, nameof(Pending), "Beklemede");
    public static readonly TransactionStatus Approved = new(2, nameof(Approved), "Onaylandı");
    public static readonly TransactionStatus Declined = new(3, nameof(Declined), "Reddedildi");
    public static readonly TransactionStatus Error = new(4, nameof(Error), "Hata");
    public static readonly TransactionStatus Timeout = new(5, nameof(Timeout), "Zaman Aşımı");
    public static readonly TransactionStatus Reversed = new(6, nameof(Reversed), "İptal Edildi");
    public static readonly TransactionStatus Settled = new(7, nameof(Settled), "Takas Edildi");

    private TransactionStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Başarılı işlem mi?
    /// </summary>
    public bool IsSuccessful => this == Approved || this == Settled;

    /// <summary>
    /// Final durum mu?
    /// </summary>
    public bool IsFinal => this == Declined || this == Error || this == Settled || this == Reversed;
}