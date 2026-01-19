using MediatR;
using MerchantSettlement.Application.Commands;
using MerchantSettlement.Application.DTOs;
using MerchantSettlement.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettlementReconciliationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettlementReconciliationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Çözülmemiş mutabakatları getirir
    /// </summary>
    [HttpGet("unresolved")]
    public async Task<ActionResult<IReadOnlyList<MerchantSettlementReconciliationDto>>> GetUnresolved(CancellationToken cancellationToken)
    {
        var query = new GetUnresolvedReconciliationsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile mutabakat getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MerchantSettlementReconciliationDto>> GetById(
        Guid id,
        [FromQuery] bool includeMismatches = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetReconciliationByIdQuery(id, includeMismatches);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Mutabakat bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Batch için mutabakat oluşturur
    /// </summary>
    [HttpPost("batch/{batchId:guid}")]
    public async Task<ActionResult<MerchantSettlementReconciliationDto>> Create(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var command = new CreateMerchantReconciliationCommand(batchId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Raporlanan tutarları ayarlar
    /// </summary>
    [HttpPost("{id:guid}/set-reported-amounts")]
    public async Task<ActionResult<MerchantSettlementReconciliationDto>> SetReportedAmounts(
        Guid id,
        [FromBody] SetReportedAmountsDto dto,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        dto.ReconciliationId = id;
        var command = new SetReportedAmountsCommand(dto, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Mutabakatı çözer
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    public async Task<ActionResult<MerchantSettlementReconciliationDto>> Resolve(
        Guid id,
        [FromQuery] string notes,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new ResolveMerchantReconciliationCommand(id, notes, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}