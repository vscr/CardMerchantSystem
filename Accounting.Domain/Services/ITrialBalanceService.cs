using Accounting.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Services;

/// <summary>
/// Mizan Servisi
/// </summary>
public interface ITrialBalanceService
{
    /// <summary>
    /// Dönem mizanı oluşturur
    /// </summary>
    Task<Result<IReadOnlyList<AccountBalance>>> GenerateTrialBalanceAsync(
        string periodCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Hesap bakiyesini günceller
    /// </summary>
    Task<Result> UpdateAccountBalanceAsync(
        Guid accountId,
        string periodCode,
        decimal debitAmount,
        decimal creditAmount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dönem bakiyelerini bir sonraki döneme taşır
    /// </summary>
    Task<Result> CarryForwardBalancesAsync(
        string fromPeriodCode,
        string toPeriodCode,
        CancellationToken cancellationToken = default);
}