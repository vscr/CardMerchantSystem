using BKM.Domain.Events;
using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Entities;

/// <summary>
/// Settlement Batch (Hesaplaşma)
/// </summary>
public class SettlementBatch : AggregateRoot
{
    public string SettlementDate { get; private set; } = null!; // YYYYMMDD format
    public string BatchNumber { get; private set; } = null!;

    // Özet Bilgiler
    public int TotalTransactionCount { get; private set; }
    public decimal TotalTransactionAmount { get; private set; }
    public decimal TotalFeeAmount { get; private set; }
    public decimal TotalNetAmount { get; private set; }

    // Durumlar
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Banka Bazlı Özet
    private readonly List<BankSettlementSummary> _bankSummaries = new();
    public IReadOnlyCollection<BankSettlementSummary> BankSummaries => _bankSummaries.AsReadOnly();

    // EF Core için
    private SettlementBatch() { }

    /// <summary>
    /// Settlement batch oluştur
    /// </summary>
    public static Result<SettlementBatch> Create(string settlementDate)
    {
        var batch = new SettlementBatch
        {
            SettlementDate = settlementDate,
            BatchNumber = $"STL{settlementDate}{new Random().Next(1000, 9999)}",
            TotalTransactionCount = 0,
            TotalTransactionAmount = 0,
            TotalFeeAmount = 0,
            TotalNetAmount = 0,
            IsCompleted = false
        };

        return batch;
    }

    /// <summary>
    /// Clearing kaydı ekle
    /// </summary>
    public Result AddClearingRecord(ClearingRecord record)
    {
        if (IsCompleted)
            return Result.Failure("Batch tamamlanmış, kayıt eklenemez");

        TotalTransactionCount++;
        TotalTransactionAmount += record.TransactionAmount;
        TotalFeeAmount += record.FeeAmount;
        TotalNetAmount += record.NetAmount;

        // Banka özetini güncelle
        UpdateBankSummary(record);

        record.MarkAsSettled(Id);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i tamamla
    /// </summary>
    public Result Complete()
    {
        if (IsCompleted)
            return Result.Failure("Batch zaten tamamlanmış");

        if (TotalTransactionCount == 0)
            return Result.Failure("Boş batch tamamlanamaz");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new SettlementCompletedEvent(
            Id,
            SettlementDate,
            TotalTransactionCount,
            TotalTransactionAmount));

        return Result.Success();
    }

    private void UpdateBankSummary(ClearingRecord record)
    {
        // Acquirer bank summary
        var acquirerSummary = _bankSummaries.FirstOrDefault(s =>
            s.BankCode == record.AcquirerBankCode && s.IsAcquirer);

        if (acquirerSummary == null)
        {
            acquirerSummary = new BankSettlementSummary(Id, record.AcquirerBankCode, true);
            _bankSummaries.Add(acquirerSummary);
        }
        acquirerSummary.AddTransaction(record.TransactionAmount, record.FeeAmount);

        // Issuer bank summary
        var issuerSummary = _bankSummaries.FirstOrDefault(s =>
            s.BankCode == record.IssuerBankCode && !s.IsAcquirer);

        if (issuerSummary == null)
        {
            issuerSummary = new BankSettlementSummary(Id, record.IssuerBankCode, false);
            _bankSummaries.Add(issuerSummary);
        }
        issuerSummary.AddTransaction(record.TransactionAmount, record.FeeAmount);
    }
}

/// <summary>
/// Banka Settlement Özeti
/// </summary>
public class BankSettlementSummary : Entity
{
    public Guid SettlementBatchId { get; private set; }
    public string BankCode { get; private set; } = null!;
    public bool IsAcquirer { get; private set; }
    public int TransactionCount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TotalFee { get; private set; }
    public decimal NetAmount { get; private set; }

    // EF Core için
    private BankSettlementSummary() { }

    public BankSettlementSummary(Guid settlementBatchId, string bankCode, bool isAcquirer)
    {
        SettlementBatchId = settlementBatchId;
        BankCode = bankCode;
        IsAcquirer = isAcquirer;
        TransactionCount = 0;
        TotalAmount = 0;
        TotalFee = 0;
        NetAmount = 0;
    }

    public void AddTransaction(decimal amount, decimal fee)
    {
        TransactionCount++;
        TotalAmount += amount;
        TotalFee += fee;

        // Acquirer alacaklı, Issuer borçlu
        NetAmount = IsAcquirer ? TotalAmount - TotalFee : -(TotalAmount - TotalFee);
    }
}