using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Mutabakat uyuşmazlık detayı
/// </summary>
public class ReconciliationMismatch : Entity
{
    public Guid ReconciliationId { get; private set; }

    // İşlem bilgileri
    public string? TransactionId { get; private set; }
    public string? TransactionNumber { get; private set; }
    public DateTime? TransactionDate { get; private set; }

    // Uyuşmazlık tipi
    public string MismatchType { get; private set; } = null!; // MissingInSystem, MissingInReport, AmountMismatch, DuplicateTransaction

    // Sistem değerleri
    public decimal? SystemAmount { get; private set; }

    // Raporlanan değerler
    public decimal? ReportedAmount { get; private set; }

    // Fark
    public decimal? AmountDifference { get; private set; }

    // Açıklama
    public string? Description { get; private set; }

    // Çözüm
    public bool IsResolved { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private ReconciliationMismatch() { }

    public static ReconciliationMismatch Create(
        Guid reconciliationId,
        string mismatchType,
        string? transactionId = null,
        string? transactionNumber = null,
        DateTime? transactionDate = null,
        decimal? systemAmount = null,
        decimal? reportedAmount = null,
        string? description = null)
    {
        return new ReconciliationMismatch
        {
            ReconciliationId = reconciliationId,
            MismatchType = mismatchType,
            TransactionId = transactionId,
            TransactionNumber = transactionNumber,
            TransactionDate = transactionDate,
            SystemAmount = systemAmount,
            ReportedAmount = reportedAmount,
            AmountDifference = (systemAmount ?? 0) - (reportedAmount ?? 0),
            Description = description,
            IsResolved = false
        };
    }

    /// <summary>
    /// Uyuşmazlığı çözer
    /// </summary>
    public void Resolve(string notes)
    {
        IsResolved = true;
        ResolutionNotes = notes;
        ResolvedAt = DateTime.UtcNow;
    }
}