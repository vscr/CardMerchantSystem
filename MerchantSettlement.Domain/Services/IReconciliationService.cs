using MerchantSettlement.Domain.Entities;

namespace MerchantSettlement.Domain.Services;

/// <summary>
/// Mutabakat servisi
/// </summary>
public interface IReconciliationService
{
    /// <summary>
    /// Batch için mutabakat oluşturur
    /// </summary>
    Task<SettlementReconciliation> CreateReconciliationAsync(
        SettlementBatch batch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raporlanan değerlerle karşılaştırma yapar
    /// </summary>
    Task<SettlementReconciliation> ReconcileAsync(
        SettlementReconciliation reconciliation,
        decimal reportedGrossAmount,
        decimal reportedCommission,
        decimal reportedNetAmount,
        int reportedTransactionCount,
        string operatorUsername,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşlem bazlı mutabakat yapar
    /// </summary>
    Task<IReadOnlyList<ReconciliationMismatch>> FindMismatchesAsync(
        SettlementBatch batch,
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