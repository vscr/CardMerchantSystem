using CardMerchantSystem.Shared.Audit.DTOs;
using CardMerchantSystem.Shared.Audit.Enums;

namespace CardMerchantSystem.Shared.Audit.Services;

/// <summary>
/// Audit loglarını sorgulama servisi
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Belirli bir entity'nin audit geçmişini getirir
    /// </summary>
    Task<IReadOnlyList<AuditLogDto>> GetEntityHistoryAsync(
        string entityName,
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir kullanıcının yaptığı işlemleri getirir
    /// </summary>
    Task<IReadOnlyList<AuditLogDto>> GetUserActionsAsync(
        string userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tarih aralığına göre audit loglarını getirir
    /// </summary>
    Task<IReadOnlyList<AuditLogDto>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        string? entityName = null,
        AuditActionType? actionType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sayfalanmış audit logları getirir
    /// </summary>
    Task<PagedAuditResult> GetPagedAsync(
        AuditLogFilter filter,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir audit log detayını getirir
    /// </summary>
    Task<AuditLogDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}