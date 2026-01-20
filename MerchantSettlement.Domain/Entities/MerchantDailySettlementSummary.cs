using CardMerchantSystem.Shared.Kernel;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Günlük takas özeti
/// Tüm merchant'ların günlük özet bilgisi
/// </summary>
public class MerchantDailySettlementSummary : AggregateRoot
{
    public DateTime SettlementDate { get; private set; }

    // Toplam değerler
    public int TotalMerchantCount { get; private set; }
    public int TotalBatchCount { get; private set; }
    public int TotalTransactionCount { get; private set; }

    // Tutarlar
    public decimal TotalSalesAmount { get; private set; }
    public decimal TotalRefundAmount { get; private set; }
    public decimal TotalChargebackAmount { get; private set; }
    public decimal TotalGrossAmount { get; private set; }
    public decimal TotalCommission { get; private set; }
    public decimal TotalFee { get; private set; }
    public decimal TotalNetAmount { get; private set; }
    public string Currency { get; private set; } = "TRY";

    // Sayaçlar
    public int SalesCount { get; private set; }
    public int RefundCount { get; private set; }
    public int ChargebackCount { get; private set; }

    // Durum sayaçları
    public int CompletedBatchCount { get; private set; }
    public int FailedBatchCount { get; private set; }
    public int PendingBatchCount { get; private set; }

    // İşlem bilgileri
    public bool IsFinalized { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public string? FinalizedBy { get; private set; }

    private MerchantDailySettlementSummary() { }

    public static MerchantDailySettlementSummary Create(DateTime settlementDate)
    {
        return new MerchantDailySettlementSummary
        {
            SettlementDate = settlementDate.Date,
            IsFinalized = false
        };
    }

    /// <summary>
    /// Batch bilgilerinden özeti günceller
    /// </summary>
    public void UpdateFromBatches(IEnumerable<MerchantSettlementBatch> batches)
    {
        if (IsFinalized)
            return;

        var batchList = batches.ToList();

        TotalMerchantCount = batchList.Select(b => b.MerchantId).Distinct().Count();
        TotalBatchCount = batchList.Count;

        TotalSalesAmount = batchList.Sum(b => b.TotalSalesAmount);
        TotalRefundAmount = batchList.Sum(b => b.TotalRefundAmount);
        TotalChargebackAmount = batchList.Sum(b => b.TotalChargebackAmount);
        TotalGrossAmount = batchList.Sum(b => b.GrossAmount);
        TotalCommission = batchList.Sum(b => b.TotalCommission);
        TotalFee = batchList.Sum(b => b.TotalFee);
        TotalNetAmount = batchList.Sum(b => b.NetAmount);

        SalesCount = batchList.Sum(b => b.SalesCount);
        RefundCount = batchList.Sum(b => b.RefundCount);
        ChargebackCount = batchList.Sum(b => b.ChargebackCount);
        TotalTransactionCount = batchList.Sum(b => b.TotalTransactionCount);

        CompletedBatchCount = batchList.Count(b => b.Status == Enums.SettlementStatus.Completed);
        FailedBatchCount = batchList.Count(b => b.Status == Enums.SettlementStatus.Failed);
        PendingBatchCount = batchList.Count(b => b.Status == Enums.SettlementStatus.Pending);
    }

    /// <summary>
    /// Günü finalize eder
    /// </summary>
    public Result Finalize(string operatorUsername)
    {
        if (IsFinalized)
            return Result.Failure("Gün zaten finalize edilmiş");

        if (PendingBatchCount > 0)
            return Result.Failure($"Bekleyen {PendingBatchCount} batch var. Önce tamamlanmalı.");

        IsFinalized = true;
        FinalizedAt = DateTime.UtcNow;
        FinalizedBy = operatorUsername;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }
}