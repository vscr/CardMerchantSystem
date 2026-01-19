using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Günsonu takas batch'i
/// Merchant'ın belirli bir dönemdeki işlemlerinin toplu takas kaydı
/// </summary>
public class MerchantSettlementBatch : AggregateRoot
{
    public string BatchNumber { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;

    // Dönem bilgileri
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public SettlementType SettlementType { get; private set; } = null!;
    public SettlementStatus Status { get; private set; } = null!;

    // Özet tutarlar
    public decimal TotalSalesAmount { get; private set; }
    public decimal TotalRefundAmount { get; private set; }
    public decimal TotalChargebackAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal TotalCommission { get; private set; }
    public decimal TotalFee { get; private set; }
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; } = "TRY";

    // Sayaçlar
    public int SalesCount { get; private set; }
    public int RefundCount { get; private set; }
    public int ChargebackCount { get; private set; }
    public int TotalTransactionCount { get; private set; }

    // İşlem bilgileri
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? FailureReason { get; private set; }

    // Detaylar
    private readonly List<MerchantSettlementDetail> _details = new();
    public IReadOnlyCollection<MerchantSettlementDetail> Details => _details.AsReadOnly();

    private MerchantSettlementBatch() { }

    public static Result<MerchantSettlementBatch> Create(
        string merchantId,
        string merchantName,
        DateTime periodStart,
        DateTime periodEnd,
        SettlementType settlementType)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantSettlementBatch>("Merchant ID boş olamaz");

        if (string.IsNullOrWhiteSpace(merchantName))
            return Result.Failure<MerchantSettlementBatch>("Merchant adı boş olamaz");

        if (periodEnd <= periodStart)
            return Result.Failure<MerchantSettlementBatch>("Dönem bitiş tarihi başlangıçtan sonra olmalı");

        var batch = new MerchantSettlementBatch
        {
            BatchNumber = GenerateBatchNumber(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            SettlementType = settlementType,
            Status = SettlementStatus.Pending
        };

        return batch;
    }

    /// <summary>
    /// Batch'e işlem detayı ekler
    /// </summary>
    public Result AddDetail(MerchantSettlementDetail detail)
    {
        if (Status != SettlementStatus.Pending)
            return Result.Failure("Sadece beklemedeki batch'e detay eklenebilir");

        _details.Add(detail);
        RecalculateTotals();
        return Result.Success();
    }

    /// <summary>
    /// Birden fazla detay ekler
    /// </summary>
    public Result AddDetails(IEnumerable<MerchantSettlementDetail> details)
    {
        if (Status != SettlementStatus.Pending)
            return Result.Failure("Sadece beklemedeki batch'e detay eklenebilir");

        _details.AddRange(details);
        RecalculateTotals();
        return Result.Success();
    }

    /// <summary>
    /// Batch'i işleme alır
    /// </summary>
    public Result StartProcessing(string processedBy)
    {
        if (!Status.CanProcess)
            return Result.Failure($"Bu durumda işlenemez. Mevcut durum: {Status.DisplayName}");

        if (_details.Count == 0)
            return Result.Failure("Batch'te işlenecek detay yok");

        Status = SettlementStatus.Processing;
        ProcessedBy = processedBy;
        MarkAsUpdated(processedBy);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i tamamlar
    /// </summary>
    public Result Complete(string operatorUsername)
    {
        if (Status != SettlementStatus.Processing)
            return Result.Failure("Sadece işlenmekte olan batch tamamlanabilir");

        Status = SettlementStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i kısmen tamamlar (bazı işlemler başarısız)
    /// </summary>
    public Result PartiallyComplete(string operatorUsername, string reason)
    {
        if (Status != SettlementStatus.Processing)
            return Result.Failure("Sadece işlenmekte olan batch kısmen tamamlanabilir");

        Status = SettlementStatus.PartiallyCompleted;
        ProcessedAt = DateTime.UtcNow;
        FailureReason = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i başarısız olarak işaretler
    /// </summary>
    public Result MarkAsFailed(string reason, string operatorUsername)
    {
        if (Status != SettlementStatus.Processing)
            return Result.Failure("Sadece işlenmekte olan batch başarısız yapılabilir");

        Status = SettlementStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Batch'i iptal eder
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = SettlementStatus.Cancelled;
        FailureReason = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    private void RecalculateTotals()
    {
        TotalSalesAmount = _details.Where(d => d.TransactionType == "Sale").Sum(d => d.Amount);
        TotalRefundAmount = _details.Where(d => d.TransactionType == "Refund").Sum(d => d.Amount);
        TotalChargebackAmount = _details.Where(d => d.TransactionType == "Chargeback").Sum(d => d.Amount);

        SalesCount = _details.Count(d => d.TransactionType == "Sale");
        RefundCount = _details.Count(d => d.TransactionType == "Refund");
        ChargebackCount = _details.Count(d => d.TransactionType == "Chargeback");
        TotalTransactionCount = _details.Count;

        GrossAmount = TotalSalesAmount - TotalRefundAmount - TotalChargebackAmount;
        TotalCommission = _details.Sum(d => d.CommissionAmount);
        TotalFee = _details.Sum(d => d.FeeAmount);
        NetAmount = GrossAmount - TotalCommission - TotalFee;
    }

    private static string GenerateBatchNumber()
    {
        return $"STL{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}