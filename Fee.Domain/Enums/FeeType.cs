using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Enums;

/// <summary>
/// Ücret Tipleri
/// </summary>
public class FeeType : Enumeration
{
    public static readonly FeeType TransactionCommission = new(1, nameof(TransactionCommission), "İşlem Komisyonu");
    public static readonly FeeType InstallmentCommission = new(2, nameof(InstallmentCommission), "Taksit Komisyonu");
    public static readonly FeeType CardAnnualFee = new(3, nameof(CardAnnualFee), "Kart Yıllık Aidat");
    public static readonly FeeType TerminalRentalFee = new(4, nameof(TerminalRentalFee), "Terminal Kiralama Ücreti");
    public static readonly FeeType POSMembershipFee = new(5, nameof(POSMembershipFee), "POS Üyelik Aidatı");
    public static readonly FeeType InterchangeFee = new(6, nameof(InterchangeFee), "Interchange Ücreti");
    public static readonly FeeType BKMFee = new(7, nameof(BKMFee), "BKM Payı");
    public static readonly FeeType WithdrawalFee = new(8, nameof(WithdrawalFee), "Nakit Çekim Ücreti");
    public static readonly FeeType ChargebackFee = new(9, nameof(ChargebackFee), "Chargeback Ücreti");
    public static readonly FeeType LateFee = new(10, nameof(LateFee), "Gecikme Faizi");

    private FeeType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Periyodik mi? (Aidat vb.)
    /// </summary>
    public bool IsPeriodic => this == CardAnnualFee ||
                               this == TerminalRentalFee ||
                               this == POSMembershipFee;

    /// <summary>
    /// İşlem bazlı mı?
    /// </summary>
    public bool IsTransactionBased => this == TransactionCommission ||
                                       this == InstallmentCommission ||
                                       this == InterchangeFee ||
                                       this == BKMFee;
}