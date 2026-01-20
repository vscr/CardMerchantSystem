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
public class FraudAlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FraudAlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// İşlenmemiş fraud uyarılarını getirir
    /// </summary>
    [HttpGet("unprocessed")]
    public async Task<ActionResult<IReadOnlyList<FraudAlertDto>>> GetUnprocessed(
        CancellationToken cancellationToken)
    {
        var query = new GetUnprocessedFraudAlertsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile fraud uyarısı getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FraudAlertDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetFraudAlertByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Fraud uyarısı bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni fraud uyarısı oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FraudAlertDto>> Create(
        [FromBody] CreateFraudAlertDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateFraudAlertCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}