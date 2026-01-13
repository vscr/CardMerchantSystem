using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Enums;

/// <summary>
/// Muhasebe İşlem Tipleri
/// </summary>
public class TransactionType : Enumeration
{
    public static readonly TransactionType CardPurchase = new(1, nameof(CardPurchase), "Kart Alışverişi");
    public static readonly TransactionType CardRefund = new(2, nameof(CardRefund), "Kart İadesi");
    public static readonly TransactionType CardPayment = new(3, nameof(CardPayment), "Kart Ödemesi");
    public static readonly TransactionType InterestAccrual = new(4, nameof(InterestAccrual), "Faiz Tahakkuku");
    public static readonly TransactionType FeeAccrual = new(5, nameof(FeeAccrual), "Ücret Tahakkuku");
    public static readonly TransactionType MerchantSettlement = new(6, nameof(MerchantSettlement), "Üye İşyeri Hakediş");
    public static readonly TransactionType CommissionIncome = new(7, nameof(CommissionIncome), "Komisyon Geliri");
    public static readonly TransactionType InterchangeIncome = new(8, nameof(InterchangeIncome), "Interchange Geliri");
    public static readonly TransactionType CashAdvance = new(9, nameof(CashAdvance), "Nakit Avans");
    public static readonly TransactionType ChargebackDebit = new(10, nameof(ChargebackDebit), "Chargeback Borç");
    public static readonly TransactionType ChargebackCredit = new(11, nameof(ChargebackCredit), "Chargeback Alacak");
    public static readonly TransactionType AnnualFee = new(12, nameof(AnnualFee), "Yıllık Aidat");
    public static readonly TransactionType LateFee = new(13, nameof(LateFee), "Gecikme Ücreti");
    public static readonly TransactionType Adjustment = new(14, nameof(Adjustment), "Düzeltme");
    public static readonly TransactionType OpeningBalance = new(15, nameof(OpeningBalance), "Açılış Bakiyesi");
    public static readonly TransactionType ClosingEntry = new(16, nameof(ClosingEntry), "Kapanış Kaydı");

    private TransactionType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}