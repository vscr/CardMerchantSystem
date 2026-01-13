using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Entities;

/// <summary>
/// Hesap Bakiyesi (Dönemsel)
/// </summary>
public class AccountBalance : Entity
{
    public Guid AccountId { get; private set; }
    public string AccountCode { get; private set; } = null!;
    public string PeriodCode { get; private set; } = null!;
    public decimal OpeningDebit { get; private set; }
    public decimal OpeningCredit { get; private set; }
    public decimal PeriodDebit { get; private set; }
    public decimal PeriodCredit { get; private set; }
    public decimal ClosingDebit { get; private set; }
    public decimal ClosingCredit { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // EF Core için
    private AccountBalance() { }

    /// <summary>
    /// Yeni hesap bakiyesi oluşturur
    /// </summary>
    public static AccountBalance Create(
        Guid accountId,
        string accountCode,
        string periodCode,
        decimal openingDebit = 0,
        decimal openingCredit = 0)
    {
        return new AccountBalance
        {
            AccountId = accountId,
            AccountCode = accountCode,
            PeriodCode = periodCode,
            OpeningDebit = openingDebit,
            OpeningCredit = openingCredit,
            PeriodDebit = 0,
            PeriodCredit = 0,
            ClosingDebit = openingDebit,
            ClosingCredit = openingCredit,
            LastUpdated = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Borç ekler
    /// </summary>
    public void AddDebit(decimal amount)
    {
        PeriodDebit += amount;
        RecalculateClosing();
    }

    /// <summary>
    /// Alacak ekler
    /// </summary>
    public void AddCredit(decimal amount)
    {
        PeriodCredit += amount;
        RecalculateClosing();
    }

    /// <summary>
    /// Borç düşer (iptal için)
    /// </summary>
    public void SubtractDebit(decimal amount)
    {
        PeriodDebit -= amount;
        RecalculateClosing();
    }

    /// <summary>
    /// Alacak düşer (iptal için)
    /// </summary>
    public void SubtractCredit(decimal amount)
    {
        PeriodCredit -= amount;
        RecalculateClosing();
    }

    private void RecalculateClosing()
    {
        ClosingDebit = OpeningDebit + PeriodDebit;
        ClosingCredit = OpeningCredit + PeriodCredit;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Net bakiye
    /// </summary>
    public decimal NetBalance => (ClosingDebit - ClosingCredit);

    /// <summary>
    /// Dönem hareketleri
    /// </summary>
    public decimal PeriodMovement => (PeriodDebit - PeriodCredit);
}