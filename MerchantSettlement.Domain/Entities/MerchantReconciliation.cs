using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Takas mutabakat kaydı
/// Banka ile merchant arasındaki mutabakat takibi
/// </summary>
public class MerchantReconciliation : AggregateRoot
{
    public string ReconciliationNumber { get; private set; } = null!;
    public Guid SettlementBatchId { get; private set; }
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;

    // Dönem
    public DateTime ReconciliationDate { get; private set; }

    // Sistem tarafındaki tutarlar
    public decimal SystemGrossAmount { get; private set; }
    public decimal SystemCommission { get; private set; }
    public decimal SystemNetAmount { get; private set; }
    public int SystemTransactionCount { get; private set; }

    // Banka/Merchant tarafındaki tutarlar
    public decimal ReportedGrossAmount { get; private set; }
    public decimal ReportedCommission { get; private set; }
    public decimal ReportedNetAmount { get; private set; }
    public int ReportedTransactionCount { get; private set; }

    // Fark tutarları
    public decimal GrossAmountDifference { get; private set; }
    public decimal CommissionDifference { get; private set; }
    public decimal NetAmountDifference { get; private set; }
    public int TransactionCountDifference { get; private set; }

    // Durum
    public ReconciliationStatus Status { get; private set; } = null!;

    // Eşleşmeyen işlemler
    private readonly List<MerchantReconciliationMismatch> _mismatches = new();
    public IReadOnlyCollection<MerchantReconciliationMismatch> Mismatches => _mismatches.AsReadOnly();

    // Çözüm bilgileri
    public string? ResolutionNotes { get; private set; }
    public string? ResolvedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private MerchantReconciliation() { }

    public static Result<MerchantReconciliation> Create(
        Guid settlementBatchId,
        string merchantId,
        string merchantName,
        DateTime reconciliationDate,
        decimal systemGrossAmount,
        decimal systemCommission,
        decimal systemNetAmount,
        int systemTransactionCount)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantReconciliation>("Merchant ID boş olamaz");

        var reconciliation = new MerchantReconciliation
        {
            ReconciliationNumber = GenerateReconciliationNumber(),
            SettlementBatchId = settlementBatchId,
            MerchantId = merchantId,
            MerchantName = merchantName,
            ReconciliationDate = reconciliationDate,
            SystemGrossAmount = systemGrossAmount,
            SystemCommission = systemCommission,
            SystemNetAmount = systemNetAmount,
            SystemTransactionCount = systemTransactionCount,
            Status = ReconciliationStatus.Pending
        };

        return reconciliation;
    }

    /// <summary>
    /// Raporlanan tutarları ayarlar ve karşılaştırma yapar
    /// </summary>
    public Result SetReportedAmounts(
        decimal reportedGrossAmount,
        decimal reportedCommission,
        decimal reportedNetAmount,
        int reportedTransactionCount,
        string operatorUsername)
    {
        if (Status.IsResolved)
            return Result.Failure("Çözülmüş mutabakat güncellenemez");

        ReportedGrossAmount = reportedGrossAmount;
        ReportedCommission = reportedCommission;
        ReportedNetAmount = reportedNetAmount;
        ReportedTransactionCount = reportedTransactionCount;

        // Farkları hesapla
        GrossAmountDifference = SystemGrossAmount - ReportedGrossAmount;
        CommissionDifference = SystemCommission - ReportedCommission;
        NetAmountDifference = SystemNetAmount - ReportedNetAmount;
        TransactionCountDifference = SystemTransactionCount - ReportedTransactionCount;

        // Durumu belirle
        DetermineStatus();

        MarkAsUpdated(operatorUsername);
        return Result.Success();
    }

    /// <summary>
    /// Eşleşmeyen işlem ekler
    /// </summary>
    public Result AddMismatch(MerchantReconciliationMismatch mismatch)
    {
        if (Status.IsResolved)
            return Result.Failure("Çözülmüş mutabakata uyuşmazlık eklenemez");

        _mismatches.Add(mismatch);

        if (Status == ReconciliationStatus.Matched)
            Status = ReconciliationStatus.Mismatched;

        return Result.Success();
    }

    /// <summary>
    /// Mutabakatı çözer
    /// </summary>
    public Result Resolve(string notes, string operatorUsername)
    {
        if (Status.IsResolved)
            return Result.Failure("Mutabakat zaten çözülmüş");

        Status = ReconciliationStatus.Resolved;
        ResolutionNotes = notes;
        ResolvedBy = operatorUsername;
        ResolvedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// İtiraz başlatır
    /// </summary>
    public Result Dispute(string reason, string operatorUsername)
    {
        if (Status.IsResolved)
            return Result.Failure("Çözülmüş mutabakata itiraz edilemez");

        Status = ReconciliationStatus.Disputed;
        ResolutionNotes = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    private void DetermineStatus()
    {
        const decimal tolerance = 0.01m; // 1 kuruş tolerans

        var isGrossMatched = Math.Abs(GrossAmountDifference) <= tolerance;
        var isCommissionMatched = Math.Abs(CommissionDifference) <= tolerance;
        var isNetMatched = Math.Abs(NetAmountDifference) <= tolerance;
        var isCountMatched = TransactionCountDifference == 0;

        if (isGrossMatched && isCommissionMatched && isNetMatched && isCountMatched)
        {
            Status = ReconciliationStatus.Matched;
        }
        else if (isNetMatched && isCountMatched)
        {
            Status = ReconciliationStatus.PartiallyMatched;
        }
        else
        {
            Status = ReconciliationStatus.Mismatched;
        }
    }

    private static string GenerateReconciliationNumber()
    {
        return $"REC{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}