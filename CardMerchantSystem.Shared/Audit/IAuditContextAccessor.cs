namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// Audit için gerekli context bilgilerine erişim sağlar.
/// HttpContext'ten user, IP, correlation ID bilgilerini alır.
/// </summary>
public interface IAuditContextAccessor
{
    /// <summary>
    /// Mevcut kullanıcının ID'si
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Mevcut kullanıcının adı
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// İstek yapan client'ın IP adresi
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    /// Request correlation ID (distributed tracing)
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Pending audit entries (Insert işlemlerinde ID henüz oluşmamış olanlar için)
    /// </summary>
    void SetPendingAuditEntries(List<AuditEntry> entries);
    List<AuditEntry>? GetPendingAuditEntries();
    void ClearPendingAuditEntries();
}