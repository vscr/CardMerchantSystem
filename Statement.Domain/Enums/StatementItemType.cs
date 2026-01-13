using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Enums;

/// <summary>
/// Ekstre Kalemi Tipleri
/// </summary>
public class StatementItemType : Enumeration
{
    public static readonly StatementItemType Purchase = new(1, nameof(Purchase), "Alışveriş");
    public static readonly StatementItemType CashAdvance = new(2, nameof(CashAdvance), "Nakit Avans");
    public static readonly StatementItemType Refund = new(3, nameof(Refund), "İade");
    public static readonly StatementItemType Payment = new(4, nameof(Payment), "Ödeme");
    public static readonly StatementItemType Interest = new(5, nameof(Interest), "Faiz");
    public static readonly StatementItemType LateFee = new(6, nameof(LateFee), "Gecikme Ücreti");
    public static readonly StatementItemType AnnualFee = new(7, nameof(AnnualFee), "Yıllık Aidat");
    public static readonly StatementItemType InsuranceFee = new(8, nameof(InsuranceFee), "Sigorta Ücreti");
    public static readonly StatementItemType ForeignExchangeFee = new(9, nameof(ForeignExchangeFee), "Döviz Kur Farkı");
    public static readonly StatementItemType Adjustment = new(10, nameof(Adjustment), "Düzeltme");
    public static readonly StatementItemType Installment = new(11, nameof(Installment), "Taksit");
    public static readonly StatementItemType Reward = new(12, nameof(Reward), "Ödül/Puan");

    private StatementItemType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Borç mu? (Pozitif tutar)
    /// </summary>
    public bool IsDebit => this == Purchase || this == CashAdvance || this == Interest ||
                           this == LateFee || this == AnnualFee || this == InsuranceFee ||
                           this == ForeignExchangeFee || this == Installment;

    /// <summary>
    /// Alacak mı? (Negatif tutar)
    /// </summary>
    public bool IsCredit => this == Refund || this == Payment || this == Reward || this == Adjustment;
}