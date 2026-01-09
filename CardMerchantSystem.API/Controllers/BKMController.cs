using BKM.Application.Commands;
using BKM.Application.DTOs;
using BKM.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BKMController : ControllerBase
{
    private readonly IMediator _mediator;

    public BKMController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authorization işlemi (Provizyon)
    /// </summary>
    [HttpPost("authorization")]
    public async Task<ActionResult<AuthorizationResponseDto>> ProcessAuthorization(
        [FromBody] AuthorizationRequestDto dto,
        CancellationToken cancellationToken)
    {
        var command = new ProcessAuthorizationCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Switch mesajını ID ile getirir
    /// </summary>
    [HttpGet("messages/{id:guid}")]
    public async Task<ActionResult<SwitchMessageDto>> GetMessageById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetSwitchMessageByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tarih aralığına göre switch mesajlarını getirir
    /// </summary>
    [HttpGet("messages")]
    public async Task<ActionResult<IReadOnlyList<SwitchMessageDto>>> GetMessagesByDate(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetSwitchMessagesByDateQuery(startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// BIN bilgisi ekler
    /// </summary>
    [HttpPost("bins")]
    public async Task<ActionResult<BINTableDto>> CreateBIN(
        [FromBody] CreateBINDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateBINCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetBINInfo), new { bin = result.Value!.BIN }, result.Value);
    }

    /// <summary>
    /// BIN bilgisini sorgular
    /// </summary>
    [HttpGet("bins/{bin}")]
    public async Task<ActionResult<BINTableDto>> GetBINInfo(
        string bin,
        CancellationToken cancellationToken)
    {
        var query = new GetBINInfoQuery(bin);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tüm aktif BIN'leri getirir
    /// </summary>
    [HttpGet("bins")]
    public async Task<ActionResult<IReadOnlyList<BINTableDto>>> GetAllBINs(
        CancellationToken cancellationToken)
    {
        var query = new GetAllBINsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Settlement işlemi yapar
    /// </summary>
    [HttpPost("settlement")]
    public async Task<ActionResult<SettlementBatchDto>> ProcessSettlement(
        [FromQuery] string? settlementDate,
        CancellationToken cancellationToken)
    {
        var date = settlementDate ?? DateTime.UtcNow.ToString("yyyyMMdd");
        var command = new ProcessSettlementCommand(date);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }
}