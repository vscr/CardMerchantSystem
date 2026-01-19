using MerchantSettlement.Domain.Entities;

namespace MerchantSettlement.Domain.Services;

/// <summary>
/// Mutabakat servisi
/// </summary>
public interface IMerchantReconciliationService
{
    /// <summary>
    /// Batch için mutabakat oluşturur
    /// </summary>
    Task<MerchantReconciliation> CreateReconciliationAsync(
        MerchantSettlementBatch batch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raporlanan değerlerle karşılaştırma yapar
    /// </summary>
    Task<MerchantReconciliation> ReconcileAsync(
        MerchantReconciliation reconciliation,
        decimal reportedGrossAmount,
        decimal reportedCommission,
        decimal reportedNetAmount,
        int reportedTransactionCount,
        string operatorUsername,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşlem bazlı mutabakat yapar
    /// </summary>
    Task<IReadOnlyList<MerchantReconciliationMismatch>> FindMismatchesAsync(
        MerchantSettlementBatch batch,
        IEnumerable<ExternalTransaction> externalTransactions,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Dış sistem işlem verisi (banka/merchant raporu)
/// </summary>
public record ExternalTransaction(
    string TransactionId,
    string TransactionNumber,
    DateTime TransactionDate,
    decimal Amount,
    string TransactionType);