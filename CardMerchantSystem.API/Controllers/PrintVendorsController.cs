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
public class PrintVendorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PrintVendorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm basım firmalarını getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PrintVendorDto>>> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllPrintVendorsQuery(activeOnly);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile basım firması getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrintVendorDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPrintVendorByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Basım firması bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni basım firması oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PrintVendorDto>> Create(
        [FromBody] CreatePrintVendorDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreatePrintVendorCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}