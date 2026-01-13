using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Entities;

/// <summary>
/// Muhasebe Fişi Satırı
/// </summary>
public class JournalEntryLine : Entity
{
    public Guid JournalEntryId { get; private set; }
    public int LineNumber { get; private set; }
    public Guid AccountId { get; private set; }
    public string AccountCode { get; private set; } = null!;
    public string AccountName { get; private set; } = null!;
    public decimal DebitAmount { get; private set; }
    public decimal CreditAmount { get; private set; }
    public string? Description { get; private set; }

    // EF Core için
    private JournalEntryLine() { }

    /// <summary>
    /// Yeni satır oluşturur
    /// </summary>
    public static Result<JournalEntryLine> Create(
        Guid journalEntryId,
        int lineNumber,
        Guid accountId,
        string accountCode,
        string accountName,
        decimal debitAmount,
        decimal creditAmount,
        string? description = null)
    {
        var line = new JournalEntryLine
        {
            JournalEntryId = journalEntryId,
            LineNumber = lineNumber,
            AccountId = accountId,
            AccountCode = accountCode,
            AccountName = accountName,
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            Description = description
        };

        return line;
    }

    /// <summary>
    /// Net tutar (Borç - Alacak)
    /// </summary>
    public decimal NetAmount => DebitAmount - CreditAmount;
}