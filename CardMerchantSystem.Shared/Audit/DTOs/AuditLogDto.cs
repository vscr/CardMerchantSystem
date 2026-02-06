using CardMerchantSystem.Shared.Audit.Enums;

namespace CardMerchantSystem.Shared.Audit.DTOs;

/// <summary>
/// Audit log sorgu sonucu DTO
/// </summary>
public record AuditLogDto
{
    public Guid Id { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public AuditActionType ActionType { get; init; }
    public string ActionTypeName => ActionType.ToString();
    public string EntityType { get; init; } = null!;
    public string EntityName { get; init; } = null!;
    public string EntityId { get; init; } = null!;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? ChangedColumns { get; init; }
    public string TableName { get; init; } = null!;
    public DateTime Timestamp { get; init; }
    public string? IpAddress { get; init; }
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Değişen alanların listesi (ChangedColumns'dan parse edilmiş)
    /// </summary>
    public List<string>? ChangedColumnsList { get; init; }

    /// <summary>
    /// Eski değerler dictionary (OldValues'dan parse edilmiş)
    /// </summary>
    public Dictionary<string, object?>? OldValuesDictionary { get; init; }

    /// <summary>
    /// Yeni değerler dictionary (NewValues'dan parse edilmiş)
    /// </summary>
    public Dictionary<string, object?>? NewValuesDictionary { get; init; }
}

/// <summary>
/// Audit log filtre DTO
/// </summary>
public record AuditLogFilter
{
    public string? EntityName { get; init; }
    public string? EntityId { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public AuditActionType? ActionType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? TableName { get; init; }
    public string? CorrelationId { get; init; }
    public string? SearchTerm { get; init; }
}

/// <summary>
/// Sayfalanmış audit sonuç DTO
/// </summary>
public record PagedAuditResult
{
    public IReadOnlyList<AuditLogDto> Items { get; init; } = Array.Empty<AuditLogDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// Entity değişiklik özeti DTO
/// </summary>
public record EntityChangeDto
{
    public string PropertyName { get; init; } = null!;
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }
}