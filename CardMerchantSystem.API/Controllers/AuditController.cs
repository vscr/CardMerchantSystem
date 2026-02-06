using CardMerchantSystem.Shared.Audit.DTOs;
using CardMerchantSystem.Shared.Audit.Enums;
using CardMerchantSystem.Shared.Audit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

/// <summary>
/// Audit log sorgulama endpoint'leri.
/// Sadece Admin ve ComplianceOfficer rolleri erişebilir.
/// </summary>
[Authorize(Policy = "ComplianceManagement")]
public class AuditController : ApiControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Sayfalanmış audit loglarını getirir
    /// </summary>
    /// <param name="filter">Filtre parametreleri</param>
    /// <param name="page">Sayfa numarası (varsayılan: 1)</param>
    /// <param name="pageSize">Sayfa boyutu (varsayılan: 20)</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedAuditResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedAuditResult>> GetPaged(
        [FromQuery] AuditLogFilter filter,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Max limit

        var result = await _auditService.GetPagedAsync(filter, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir audit log detayını getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditLogDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _auditService.GetByIdAsync(id, cancellationToken);
        return OkOrNotFound(result, "Audit log", id);
    }

    /// <summary>
    /// Belirli bir entity'nin değişiklik geçmişini getirir
    /// </summary>
    /// <param name="entityName">Entity adı (örn: Merchant, Transaction)</param>
    /// <param name="entityId">Entity ID</param>
    [HttpGet("entity/{entityName}/{entityId}")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetEntityHistory(
        string entityName,
        string entityId,
        CancellationToken cancellationToken)
    {
        var result = await _auditService.GetEntityHistoryAsync(entityName, entityId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir kullanıcının yaptığı işlemleri getirir
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <param name="from">Başlangıç tarihi (isteğe bağlı)</param>
    /// <param name="to">Bitiş tarihi (isteğe bağlı)</param>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetUserActions(
        string userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var result = await _auditService.GetUserActionsAsync(userId, from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Tarih aralığına göre audit loglarını getirir
    /// </summary>
    /// <param name="from">Başlangıç tarihi</param>
    /// <param name="to">Bitiş tarihi</param>
    /// <param name="entityName">Entity adı (isteğe bağlı)</param>
    /// <param name="actionType">İşlem tipi (isteğe bağlı)</param>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] string? entityName = null,
        [FromQuery] AuditActionType? actionType = null,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
            return BadRequest(new ApiErrorResponse("Başlangıç tarihi bitiş tarihinden büyük olamaz", "INVALID_DATE_RANGE"));

        // Max 30 günlük aralık
        if ((to - from).TotalDays > 30)
            return BadRequest(new ApiErrorResponse("Maksimum 30 günlük aralık sorgulanabilir", "DATE_RANGE_TOO_LARGE"));

        var result = await _auditService.GetByDateRangeAsync(from, to, entityName, actionType, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Desteklenen entity tiplerini listeler
    /// </summary>
    [HttpGet("entity-types")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetEntityTypes()
    {
        // Projede mevcut entity'ler
        var entityTypes = new List<string>
        {
            "Merchant",
            "Terminal",
            "Transaction",
            "CardApplication",
            "CardBlock",
            "DisputeAggregate",
            "CampaignAggregate",
            "WorkOrder",
            "FraudAlert",
            "MerchantSettlementBatch",
            "JournalEntry",
            "ReportRequest",
            "Shipment"
            // Diğer entity'ler eklenebilir
        };

        return Ok(entityTypes);
    }

    /// <summary>
    /// Audit işlem tiplerini listeler
    /// </summary>
    [HttpGet("action-types")]
    [ProducesResponseType(typeof(IReadOnlyList<object>), StatusCodes.Status200OK)]
    public IActionResult GetActionTypes()
    {
        var actionTypes = Enum.GetValues<AuditActionType>()
            .Select(x => new { Id = (int)x, Name = x.ToString() })
            .ToList();

        return Ok(actionTypes);
    }
}