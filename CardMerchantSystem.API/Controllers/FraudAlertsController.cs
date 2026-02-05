using EarlyBlockResolution.Application.Commands;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class FraudAlertsController : ApiControllerBase
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

        return OkOrNotFound(result, "Fraud uyarısı", id);
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

        return CreatedOrBadRequest(result, nameof(GetById), x => new { id = x.Id });
    }
}