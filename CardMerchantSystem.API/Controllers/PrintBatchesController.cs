using MediatR;
using BulkCardPrint.Application.Commands;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrintBatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PrintBatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Bekleyen basım batch'lerini getirir
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<PrintBatchDto>>> GetPending(CancellationToken cancellationToken)
    {
        var query = new GetPendingPrintBatchesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Duruma göre batch'leri getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<PrintBatchDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrintBatchesByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile batch getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrintBatchDto>> GetById(
        Guid id,
        [FromQuery] bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPrintBatchByIdQuery(id, includeItems);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Batch bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni basım batch'i oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PrintBatchDto>> Create(
        [FromBody] CreatePrintBatchDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreatePrintBatchCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Batch'e item ekler
    /// </summary>
    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<PrintBatchDto>> AddItems(
        Guid id,
        [FromBody] List<AddPrintBatchItemDto> items,
        CancellationToken cancellationToken)
    {
        var command = new AddItemsToPrintBatchCommand(id, items);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Batch için dosya oluşturur
    /// </summary>
    [HttpPost("{id:guid}/generate-file")]
    public async Task<ActionResult<PrintBatchDto>> GenerateFile(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new GeneratePrintFileCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Batch'i firmaya gönderir
    /// </summary>
    [HttpPost("{id:guid}/send-to-vendor")]
    public async Task<ActionResult<PrintBatchDto>> SendToVendor(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new SendBatchToVendorCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Üretimi başlatır
    /// </summary>
    [HttpPost("{id:guid}/start-production")]
    public async Task<ActionResult<PrintBatchDto>> StartProduction(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new StartBatchProductionCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Batch'i tamamlar
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<PrintBatchDto>> Complete(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new CompletePrintBatchCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}