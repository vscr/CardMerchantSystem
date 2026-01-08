using Transaction.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Services;

/// <summary>
/// Limit Kontrol Sistemi (LKS) Domain Service Interface
/// </summary>
public interface ILimitService
{
    /// <summary>
    /// Kart limitlerini getirir
    /// </summary>
    Task<Result<CardLimit>> GetCardLimitAsync(string cardNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limit kontrolü yapar
    /// </summary>
    Task<Result> CheckLimitAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limit kullanımı rezerve eder (Pre-auth)
    /// </summary>
    Task<Result> ReserveLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limit kullanımını onaylar
    /// </summary>
    Task<Result> CommitLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rezerve edilen limiti serbest bırakır
    /// </summary>
    Task<Result> ReleaseLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limit iadesi yapar (İade/İptal)
    /// </summary>
    Task<Result> RefundLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Günlük limitleri sıfırlar
    /// </summary>
    Task<Result> ResetDailyLimitsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Aylık limitleri sıfırlar
    /// </summary>
    Task<Result> ResetMonthlyLimitsAsync(CancellationToken cancellationToken = default);
}