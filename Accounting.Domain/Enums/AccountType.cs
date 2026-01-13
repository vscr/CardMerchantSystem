using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Enums;

/// <summary>
/// Hesap Tipleri
/// </summary>
public class AccountType : Enumeration
{
    public static readonly AccountType Asset = new(1, nameof(Asset), "Varlık");
    public static readonly AccountType Liability = new(2, nameof(Liability), "Yükümlülük");
    public static readonly AccountType Equity = new(3, nameof(Equity), "Özkaynak");
    public static readonly AccountType Revenue = new(4, nameof(Revenue), "Gelir");
    public static readonly AccountType Expense = new(5, nameof(Expense), "Gider");

    private AccountType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Normal bakiye borç tarafında mı?
    /// </summary>
    public bool IsDebitNormal => this == Asset || this == Expense;

    /// <summary>
    /// Normal bakiye alacak tarafında mı?
    /// </summary>
    public bool IsCreditNormal => this == Liability || this == Equity || this == Revenue;
}