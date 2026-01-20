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
public class MerchantPayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MerchantPayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm bekleyen ödemeleri getirir
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<MerchantPayoutDto>>> GetPending(CancellationToken cancellationToken)
    {
        var query = new GetPendingPayoutsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Merchant'a göre ödemeleri getirir
    /// </summary>
    [HttpGet("by-merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<MerchantPayoutDto>>> GetByMerchant(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantPayoutsByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile ödeme getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MerchantPayoutDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantPayoutByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Ödeme bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Yeni ödeme oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MerchantPayoutDto>> Create(
        [FromBody] CreateMerchantPayoutDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateMerchantPayoutCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Ödemeyi planlar
    /// </summary>
    [HttpPost("{id:guid}/schedule")]
    public async Task<ActionResult<MerchantPayoutDto>> Schedule(
        Guid id,
        [FromQuery] DateTime scheduledDate,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new SchedulePayoutCommand(id, scheduledDate, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Ödemeyi tamamlar
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<MerchantPayoutDto>> Complete(
        Guid id,
        [FromQuery] string bankReferenceNumber,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new CompletePayoutCommand(id, bankReferenceNumber, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}