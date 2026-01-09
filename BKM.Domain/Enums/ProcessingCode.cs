using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Enums;

/// <summary>
/// ISO 8583 Processing Kodları
/// </summary>
public class ProcessingCode : Enumeration
{
    public static readonly ProcessingCode Purchase = new(1, "000000", "Satış");
    public static readonly ProcessingCode CashAdvance = new(2, "010000", "Nakit Avans");
    public static readonly ProcessingCode Refund = new(3, "200000", "İade");
    public static readonly ProcessingCode BalanceInquiry = new(4, "300000", "Bakiye Sorgulama");
    public static readonly ProcessingCode PaymentFromAccount = new(5, "400000", "Hesaptan Ödeme");
    public static readonly ProcessingCode PreAuthorization = new(6, "030000", "Ön Provizyon");
    public static readonly ProcessingCode PreAuthCompletion = new(7, "000000", "Ön Provizyon Kapama");

    private ProcessingCode(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Hesaptan para çeker mi?
    /// </summary>
    public bool IsDebit => this == Purchase || this == CashAdvance || this == PreAuthorization;

    /// <summary>
    /// Hesaba para ekler mi?
    /// </summary>
    public bool IsCredit => this == Refund;
}