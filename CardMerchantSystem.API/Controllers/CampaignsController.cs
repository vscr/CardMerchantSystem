using Campaign.Application.Commands;
using Campaign.Application.DTOs;
using Campaign.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni kampanya oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CampaignDto>> Create(
        [FromBody] CreateCampaignDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCampaignCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// ID ile kampanya getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CampaignDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Kampanya detaylarını getirir (kurallar ve kullanımlarla birlikte)
    /// </summary>
    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<CampaignDetailDto>> GetDetails(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignDetailQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Aktif kampanyaları getirir
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<CampaignDto>>> GetActive(
        CancellationToken cancellationToken)
    {
        var query = new GetActiveCampaignsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Kampanyayı aktif eder
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        [FromQuery] string approverUsername,
        CancellationToken cancellationToken)
    {
        var command = new ActivateCampaignCommand(id, approverUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Kampanya aktif edildi" });
    }

    /// <summary>
    /// Kampanyayı duraklatır
    /// </summary>
    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause(
        Guid id,
        [FromQuery] string reason,
        [FromQuery] string username,
        CancellationToken cancellationToken)
    {
        var command = new PauseCampaignCommand(id, reason, username);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Kampanya duraklatıldı" });
    }

    /// <summary>
    /// Kampanyaya kural ekler
    /// </summary>
    [HttpPost("{id:guid}/rules")]
    public async Task<IActionResult> AddRule(
        Guid id,
        [FromBody] AddRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCampaignRuleCommand(id, request.RuleName, request.RuleType, request.Operator, request.Value);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Kural eklendi" });
    }

    /// <summary>
    /// Kampanyayı işleme uygular
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<ApplyCampaignResultDto>> Apply(
        [FromBody] ApplyCampaignDto dto,
        CancellationToken cancellationToken)
    {
        var command = new ApplyCampaignCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// İndirim önizleme hesaplar
    /// </summary>
    [HttpGet("calculate-discount")]
    public async Task<ActionResult<DiscountPreviewDto>> CalculateDiscount(
        [FromQuery] string campaignCode,
        [FromQuery] decimal transactionAmount,
        CancellationToken cancellationToken)
    {
        var query = new CalculateDiscountQuery(campaignCode, transactionAmount);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }
}

// Request DTOs
public class AddRuleRequest
{
    public string RuleName { get; set; } = null!;
    public string RuleType { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string Value { get; set; } = null!;
}