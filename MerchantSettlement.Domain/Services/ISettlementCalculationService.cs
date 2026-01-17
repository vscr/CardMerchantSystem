using MerchantSettlement.Domain.Entities;

namespace MerchantSettlement.Domain.Services;

/// <summary>
/// Takas hesaplama servisi
/// </summary>
public interface ISettlementCalculationService
{
    /// <summary>
    /// Merchant için günlük takas hesaplar
    /// </summary>
    Task<SettlementBatch> CalculateDailySettlementAsync(
        string merchantId,
        string merchantName,
        DateTime settlementDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Hakediş hesaplar
    /// </summary>
    MerchantPayout CalculatePayout(
        string merchantId,
        string merchantName,
        string bankCode,
        string bankName,
        string iban,
        IEnumerable<SettlementBatch> batches,
        decimal withholdingTaxRate = 0);

    /// <summary>
    /// Komisyon hesaplar
    /// </summary>
    decimal CalculateCommission(decimal amount, decimal commissionRate);

    /// <summary>
    /// Stopaj hesaplar
    /// </summary>
    decimal CalculateWithholdingTax(decimal netAmount, decimal taxRate);
}