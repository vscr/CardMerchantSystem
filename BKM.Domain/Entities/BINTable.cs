using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Entities;

/// <summary>
/// BIN Tablosu - Kart numarasından banka belirleme
/// </summary>
public class BINTable : Entity
{
    public string BIN { get; private set; } = null!;
    public string BankCode { get; private set; } = null!;
    public string BankName { get; private set; } = null!;
    public string CardBrand { get; private set; } = null!; // Visa, Mastercard, Troy
    public string CardType { get; private set; } = null!; // Credit, Debit, Prepaid
    public string CardLevel { get; private set; } = null!; // Classic, Gold, Platinum
    public bool IsActive { get; private set; }

    // EF Core için
    private BINTable() { }

    public static BINTable Create(
        string bin,
        string bankCode,
        string bankName,
        string cardBrand,
        string cardType,
        string cardLevel)
    {
        return new BINTable
        {
            BIN = bin,
            BankCode = bankCode,
            BankName = bankName,
            CardBrand = cardBrand,
            CardType = cardType,
            CardLevel = cardLevel,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}