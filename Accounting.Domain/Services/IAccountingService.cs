using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Services;

/// <summary>
/// Muhasebe Servisi
/// </summary>
public interface IAccountingService
{
    /// <summary>
    /// Kart alışverişi muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostCardPurchaseAsync(
        Guid transactionId,
        string cardNumber,
        decimal amount,
        string merchantName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kart iadesi muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostCardRefundAsync(
        Guid transactionId,
        string cardNumber,
        decimal amount,
        string merchantName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kart ödemesi muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostCardPaymentAsync(
        Guid statementId,
        string cardNumber,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Faiz tahakkuku muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostInterestAccrualAsync(
        Guid statementId,
        string cardNumber,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Komisyon geliri muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostCommissionIncomeAsync(
        Guid transactionId,
        string merchantId,
        decimal totalCommission,
        decimal bankShare,
        decimal interchangeFee,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Üye işyeri hakediş muhasebeleştirir
    /// </summary>
    Task<Result<JournalEntry>> PostMerchantSettlementAsync(
        string merchantId,
        decimal grossAmount,
        decimal commission,
        decimal netAmount,
        CancellationToken cancellationToken = default);
}