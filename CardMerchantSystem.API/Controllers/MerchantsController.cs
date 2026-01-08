using Merchant.Application.Commands;
using Merchant.Application.DTOs;
using Merchant.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MerchantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MerchantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni üye işyeri oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MerchantDto>> Create(
        [FromBody] CreateMerchantDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateMerchantCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// ID ile üye işyeri getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MerchantDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Üye işyeri ve terminalleri getirir
    /// </summary>
    [HttpGet("{id:guid}/terminals")]
    public async Task<ActionResult<MerchantWithTerminalsDto>> GetWithTerminals(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantWithTerminalsQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Duruma göre üye işyerlerini getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<MerchantDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantsByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Üye işyerini onaylar
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromQuery] string approverUsername,
        CancellationToken cancellationToken)
    {
        var command = new ApproveMerchantCommand(id, approverUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Üye işyeri onaylandı" });
    }

    /// <summary>
    /// Üye işyerini aktif eder
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new ActivateMerchantCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Üye işyeri aktif edildi" });
    }

    /// <summary>
    /// Üye işyerine terminal ekler
    /// </summary>
    [HttpPost("{id:guid}/terminals")]
    public async Task<ActionResult<TerminalDto>> AddTerminal(
        Guid id,
        [FromBody] AddTerminalDto dto,
        CancellationToken cancellationToken)
    {
        var command = new AddTerminalCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Terminali aktif eder
    /// </summary>
    [HttpPost("{merchantId:guid}/terminals/{terminalId:guid}/activate")]
    public async Task<IActionResult> ActivateTerminal(
        Guid merchantId,
        Guid terminalId,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new ActivateTerminalCommand(merchantId, terminalId, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Terminal aktif edildi" });
    }
}