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
public class SettlementBatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettlementBatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm bekleyen batch'leri getirir
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<SettlementBatchDto>>> GetPending(CancellationToken cancellationToken)
    {
        var query = new GetPendingSettlementBatchesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Merchant'a göre batch'leri getirir
    /// </summary>
    [HttpGet("by-merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<SettlementBatchDto>>> GetByMerchant(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetSettlementBatchesByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile batch getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SettlementBatchDto>> GetById(
        Guid id,
        [FromQuery] bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSettlementBatchByIdQuery(id, includeDetails);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Batch bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni batch oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SettlementBatchDto>> Create(
        [FromBody] CreateSettlementBatchDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateSettlementBatchCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Batch'e detay ekler
    /// </summary>
    [HttpPost("{id:guid}/details")]
    public async Task<ActionResult<SettlementBatchDto>> AddDetails(
        Guid id,
        [FromBody] List<AddSettlementDetailDto> details,
        CancellationToken cancellationToken)
    {
        var command = new AddSettlementDetailsCommand(id, details);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Batch'i işler
    /// </summary>
    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<SettlementBatchDto>> Process(
        Guid id,
        [FromQuery] string processedBy,
        CancellationToken cancellationToken)
    {
        var command = new ProcessSettlementBatchCommand(id, processedBy);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}