using Statement.Domain.Entities;
using Statement.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Services;

/// <summary>
/// Ekstre Bildirim Servisi
/// </summary>
public interface IStatementNotificationService
{
    /// <summary>
    /// Ekstre bildirimi gönderir
    /// </summary>
    Task<Result> SendNotificationAsync(
        CardStatement statement,
        NotificationType notificationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ödeme hatırlatması gönderir
    /// </summary>
    Task<Result> SendPaymentReminderAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gecikme bildirimi gönderir
    /// </summary>
    Task<Result> SendOverdueNotificationAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default);
}