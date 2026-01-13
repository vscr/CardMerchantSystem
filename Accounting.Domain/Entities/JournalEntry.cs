using Accounting.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Entities;

/// <summary>
/// Muhasebe Fişi
/// </summary>
public class JournalEntry : AggregateRoot
{
    public string EntryNumber { get; private set; } = null!;
    public DateTime EntryDate { get; private set; }
    public string PeriodCode { get; private set; } = null!;
    public TransactionType TransactionType { get; private set; } = null!;
    public JournalEntryStatus Status { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? ReferenceNumber { get; private set; }
    public string? ReferenceType { get; private set; } // Transaction, Statement, Fee vb.
    public Guid? ReferenceId { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public Guid? ReversedEntryId { get; private set; }

    // Toplamlar
    public decimal TotalDebit { get; private set; }
    public decimal TotalCredit { get; private set; }

    // İlişkili satırlar
    private readonly List<JournalEntryLine> _lines = new();
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.AsReadOnly();

    // EF Core için
    private JournalEntry() { }

    /// <summary>
    /// Yeni muhasebe fişi oluşturur
    /// </summary>
    public static Result<JournalEntry> Create(
        DateTime entryDate,
        string periodCode,
        TransactionType transactionType,
        string description,
        string? referenceNumber = null,
        string? referenceType = null,
        Guid? referenceId = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<JournalEntry>("Açıklama boş olamaz");

        var entry = new JournalEntry
        {
            EntryNumber = GenerateEntryNumber(),
            EntryDate = entryDate,
            PeriodCode = periodCode,
            TransactionType = transactionType,
            Status = JournalEntryStatus.Draft,
            Description = description,
            ReferenceNumber = referenceNumber,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            TotalDebit = 0,
            TotalCredit = 0
        };

        return entry;
    }

    /// <summary>
    /// Satır ekler
    /// </summary>
    public Result<JournalEntryLine> AddLine(
        Guid accountId,
        string accountCode,
        string accountName,
        decimal debitAmount,
        decimal creditAmount,
        string? description = null)
    {
        if (!Status.IsEditable)
            return Result.Failure<JournalEntryLine>("Onaylanmış fişe satır eklenemez");

        if (debitAmount < 0 || creditAmount < 0)
            return Result.Failure<JournalEntryLine>("Tutar negatif olamaz");

        if (debitAmount > 0 && creditAmount > 0)
            return Result.Failure<JournalEntryLine>("Aynı satırda hem borç hem alacak olamaz");

        if (debitAmount == 0 && creditAmount == 0)
            return Result.Failure<JournalEntryLine>("Borç veya alacak tutarı girilmeli");

        var lineNumber = _lines.Count + 1;

        var line = JournalEntryLine.Create(
            Id,
            lineNumber,
            accountId,
            accountCode,
            accountName,
            debitAmount,
            creditAmount,
            description);

        if (line.IsFailure)
            return Result.Failure<JournalEntryLine>(line.Error!);

        _lines.Add(line.Value!);
        RecalculateTotals();

        return line.Value!;
    }

    /// <summary>
    /// Fişi onayla
    /// </summary>
    public Result Post(string postedBy)
    {
        if (!Status.IsEditable)
            return Result.Failure("Fiş zaten onaylanmış");

        if (!_lines.Any())
            return Result.Failure("En az bir satır eklenmeli");

        if (TotalDebit != TotalCredit)
            return Result.Failure($"Borç ({TotalDebit:N2}) ve Alacak ({TotalCredit:N2}) eşit olmalı");

        Status = JournalEntryStatus.Posted;
        PostedAt = DateTime.UtcNow;
        PostedBy = postedBy;

        return Result.Success();
    }

    /// <summary>
    /// Fişi iptal et (ters kayıt oluşturur)
    /// </summary>
    public Result<JournalEntry> Reverse(string periodCode, string reversedBy)
    {
        if (!Status.IsPosted)
            return Result.Failure<JournalEntry>("Sadece onaylanmış fişler iptal edilebilir");

        if (ReversedEntryId.HasValue)
            return Result.Failure<JournalEntry>("Fiş zaten iptal edilmiş");

        // Ters kayıt oluştur
        var reverseEntry = new JournalEntry
        {
            EntryNumber = GenerateEntryNumber(),
            EntryDate = DateTime.UtcNow,
            PeriodCode = periodCode,
            TransactionType = TransactionType,
            Status = JournalEntryStatus.Draft,
            Description = $"İPTAL: {Description}",
            ReferenceNumber = EntryNumber,
            ReferenceType = "Reversal",
            ReferenceId = Id
        };

        // Satırları ters çevir
        foreach (var line in _lines)
        {
            reverseEntry.AddLine(
                line.AccountId,
                line.AccountCode,
                line.AccountName,
                line.CreditAmount, // Borç <-> Alacak değiş tokuş
                line.DebitAmount,
                $"İPTAL: {line.Description}");
        }

        // Onayla
        reverseEntry.Post(reversedBy);

        // Bu fişi iptal edildi olarak işaretle
        Status = JournalEntryStatus.Reversed;
        ReversedEntryId = reverseEntry.Id;

        return reverseEntry;
    }

    private void RecalculateTotals()
    {
        TotalDebit = _lines.Sum(x => x.DebitAmount);
        TotalCredit = _lines.Sum(x => x.CreditAmount);
    }

    /// <summary>
    /// Fiş dengeli mi?
    /// </summary>
    public bool IsBalanced => TotalDebit == TotalCredit;

    private static string GenerateEntryNumber()
    {
        return $"JE{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }
}