using CardMerchantSystem.API.Auth.Constants;
using DocumentFormat.OpenXml.Wordprocessing;
using Fraud.Application.Commands;
using Fraud.Application.Queries;
using Fraud.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.Fraud;

[Authorize]
public class FraudAlertsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public FraudAlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════
    // ALERTS
    // ═══════════════════════════════════════

    /// <summary>
    /// Fraud alert listesi (filtrelenebilir, sayfalanabilir)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult<FraudAlertsResult>> GetAlerts(
        [FromQuery] int? status,
        [FromQuery] string? assignedTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetFraudAlertsQuery(
            status.HasValue ? (FraudAlertStatus)status.Value : null,
            assignedTo, page, pageSize);

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Tek alert detayı (hit scenarios + actions + kart profili)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult<FraudAlertDetailDto>> GetAlertDetail(
        Guid id, CancellationToken ct = default)
    {
        var query = new GetFraudAlertDetailQuery(id);
        var result = await _mediator.Send(query, ct);
        return OkOrNotFound(result, "Fraud Alert", id);
    }

    /// <summary>
    /// Alert'i operatöre ata
    /// </summary>
    [HttpPut("{id:guid}/assign")]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult> AssignAlert(
        Guid id, [FromBody] AssignAlertRequest request, CancellationToken ct = default)
    {
        var command = new AssignFraudAlertCommand(id, request.AssignTo);
        var result = await _mediator.Send(command, ct);

        if (!result) return NotFound(new { error = "Alert bulunamadı" });
        return Ok("Alert atandı");
    }

    /// <summary>
    /// Alert'i çözümle (karar ver + aksiyon al)
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult> ResolveAlert(
        Guid id, [FromBody] ResolveAlertRequest request, CancellationToken ct = default)
    {
        var command = new ResolveFraudAlertCommand(
            id, request.Decision, request.Comment,
            request.CardStatusAction, request.CardStatusReasonCode,
            CurrentUsername ?? "system");

        var actionId = await _mediator.Send(command, ct);
        return Ok(new { actionId, message = "Alert çözümlendi" });
    }

    /// <summary>
    /// Fraud dashboard — KPI'lar
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult<FraudDashboardDto>> GetDashboard(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFraudDashboardQuery(), ct);
        return Ok(result);
    }
}

// ── Request DTOs ──

public class AssignAlertRequest
{
    public string AssignTo { get; set; } = null!;
}

public class ResolveAlertRequest
{
    public FraudDecision Decision { get; set; }
    public string? Comment { get; set; }
    public string? CardStatusAction { get; set; }
    public string? CardStatusReasonCode { get; set; }
}