using EarlyBlockResolution.Application.Commands;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardBlocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public CardBlocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Doğrulama bekleyen blokları getirir
    /// </summary>
    [HttpGet("pending-verification")]
    public async Task<ActionResult<IReadOnlyList<CardBlockDto>>> GetPendingVerification(
        CancellationToken cancellationToken)
    {
        var query = new GetPendingVerificationBlocksQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Duruma göre blokları getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<CardBlockDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetCardBlocksByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile blok getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CardBlockDto>> GetById(
        Guid id,
        [FromQuery] bool includeVerifications = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCardBlockByIdQuery(id, includeVerifications);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Blok bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni kart bloğu oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CardBlockDto>> Create(
        [FromBody] CreateCardBlockDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCardBlockCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Doğrulama başlatır
    /// </summary>
    [HttpPost("{id:guid}/initiate-verification")]
    public async Task<ActionResult<BlockVerificationDto>> InitiateVerification(
        Guid id,
        [FromBody] InitiateVerificationDto dto,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new InitiateVerificationCommand(id, dto, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// OTP doğrular
    /// </summary>
    [HttpPost("{id:guid}/verify-otp")]
    public async Task<ActionResult<BlockVerificationDto>> VerifyOtp(
        Guid id,
        [FromBody] VerifyOtpDto dto,
        CancellationToken cancellationToken)
    {
        var command = new VerifyOtpCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Bloğu çözer
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    public async Task<ActionResult<CardBlockDto>> Resolve(
        Guid id,
        [FromQuery] string resolvedBy,
        [FromQuery] string? resolutionNotes,
        CancellationToken cancellationToken)
    {
        var command = new ResolveBlockCommand(id, resolvedBy, resolutionNotes);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Bloğu üst seviyeye iletir
    /// </summary>
    [HttpPost("{id:guid}/escalate")]
    public async Task<ActionResult<CardBlockDto>> Escalate(
        Guid id,
        [FromQuery] string reason,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new EscalateBlockCommand(id, reason, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}