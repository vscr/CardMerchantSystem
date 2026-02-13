using CardMerchantSystem.API.Auth.Constants;
using Fraud.Application.Commands;
using Fraud.Application.Queries;
using Fraud.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.Fraud;

[Authorize(Policy = Policies.AdminOnly)]
public class FraudScenariosController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public FraudScenariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════
    // SCENARIOS
    // ═══════════════════════════════════════

    /// <summary>
    /// Tüm fraud senaryolarını listele
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<FraudScenarioDto>>> GetScenarios(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFraudScenariosQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni senaryo oluştur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateScenario(
        [FromBody] CreateScenarioRequest request, CancellationToken ct = default)
    {
        var command = new CreateFraudScenarioCommand(
            request.Name, request.Description,
            request.RuleId, request.FilterRuleId,
            request.CheckMode, request.FraudResponseCode,
            request.Score, request.RunOrder, request.IsSimulation,
            request.StartDate, request.EndDate,
            CurrentUsername ?? "system");

        var id = await _mediator.Send(command, ct);
        return Ok(new { id, message = "Senaryo oluşturuldu" });
    }

    // ═══════════════════════════════════════
    // RULES
    // ═══════════════════════════════════════

    /// <summary>
    /// Tüm fraud kurallarını listele
    /// </summary>
    [HttpGet("rules")]
    public async Task<ActionResult<List<FraudRuleDto>>> GetRules(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFraudRulesQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Yeni kural oluştur
    /// </summary>
    [HttpPost("rules")]
    public async Task<ActionResult> CreateRule(
        [FromBody] CreateRuleRequest request, CancellationToken ct = default)
    {
        var command = new CreateFraudRuleCommand(
            request.Code, request.Name, request.Description,
            request.RuleType, request.ConditionOperator,
            CurrentUsername ?? "system",
            request.PeriodMinutes, request.PeriodThreshold,
            request.PeriodFunction, request.PeriodGroupBy,
            request.SqlScript, request.Conditions);

        var id = await _mediator.Send(command, ct);
        return Ok(new { id, message = "Kural oluşturuldu" });
    }

    // ═══════════════════════════════════════
    // BLACKLIST
    // ═══════════════════════════════════════

    /// <summary>
    /// Kara/beyaz listeye ekle
    /// </summary>
    [HttpPost("blacklist")]
    public async Task<ActionResult> AddToBlacklist(
        [FromBody] AddToBlacklistRequest request, CancellationToken ct = default)
    {
        var command = new AddToBlacklistCommand(
            request.ListType, request.Value, request.IsBlacklist,
            request.Reason, request.ExpiresAt,
            CurrentUsername ?? "system");

        var id = await _mediator.Send(command, ct);
        return Ok(new { id, message = $"{(request.IsBlacklist ? "Kara" : "Beyaz")} listeye eklendi" });
    }

    // ═══════════════════════════════════════
    // CARD PROFILE
    // ═══════════════════════════════════════

    /// <summary>
    /// Kart fraud profili
    /// </summary>
    [HttpGet("card-profile/{maskedCardNo}")]
    [Authorize(Roles = RoleNames.FraudTeam)]
    public async Task<ActionResult<CardFraudProfileDetailDto>> GetCardProfile(
        string maskedCardNo, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCardFraudProfileQuery(maskedCardNo), ct);
        return OkOrNotFound(result, "Kart Fraud Profili", maskedCardNo);
    }
}

// ── Request DTOs ──

public class CreateScenarioRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid RuleId { get; set; }
    public Guid? FilterRuleId { get; set; }
    public FraudCheckMode CheckMode { get; set; }
    public string FraudResponseCode { get; set; } = null!;
    public int Score { get; set; }
    public int RunOrder { get; set; }
    public bool IsSimulation { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateRuleRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public FraudRuleType RuleType { get; set; }
    public LogicalOperator ConditionOperator { get; set; }
    public int? PeriodMinutes { get; set; }
    public decimal? PeriodThreshold { get; set; }
    public string? PeriodFunction { get; set; }
    public string? PeriodGroupBy { get; set; }
    public string? SqlScript { get; set; }
    public List<CreateRuleConditionDto>? Conditions { get; set; }
}

public class AddToBlacklistRequest
{
    public string ListType { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsBlacklist { get; set; } = true;
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
}