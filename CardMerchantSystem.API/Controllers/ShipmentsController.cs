using Courier.Application.Commands;
using Courier.Application.DTOs;
using Courier.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class ShipmentsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Bekleyen teslimatları getirir
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetPending(CancellationToken cancellationToken)
    {
        var query = new GetPendingDeliveriesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Başarısız teslimatları getirir
    /// </summary>
    [HttpGet("failed")]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetFailed(CancellationToken cancellationToken)
    {
        var query = new GetFailedDeliveriesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Duruma göre gönderileri getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetShipmentsByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Takip numarası ile gönderi getirir
    /// </summary>
    [HttpGet("track/{trackingNumber}")]
    public async Task<ActionResult<ShipmentDto>> GetByTrackingNumber(
        string trackingNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetShipmentByTrackingNumberQuery(trackingNumber);
        var result = await _mediator.Send(query, cancellationToken);

        return OkOrNotFound(result, "Gönderi", trackingNumber);
    }

    /// <summary>
    /// ID ile gönderi getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetById(
        Guid id,
        [FromQuery] bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetShipmentByIdQuery(id, includeDetails);
        var result = await _mediator.Send(query, cancellationToken);

        return OkOrNotFound(result, "Gönderi", id);
    }

    /// <summary>
    /// Yeni gönderi oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(
        [FromBody] CreateShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateShipmentCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetById), x => new { id = x.Id });
    }

    /// <summary>
    /// Gönderiyi kurye tarafından alındı olarak işaretle
    /// </summary>
    [HttpPost("{id:guid}/pickup")]
    public async Task<ActionResult<ShipmentDto>> PickUp(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new PickUpShipmentCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Gönderi durumunu güncelle
    /// </summary>
    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<ShipmentDto>> UpdateStatus(
        Guid id,
        [FromQuery] string statusAction,
        [FromQuery] string? location,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new UpdateShipmentStatusCommand(id, statusAction, location, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Gönderiyi teslim edildi olarak işaretle
    /// </summary>
    [HttpPost("{id:guid}/deliver")]
    public async Task<ActionResult<ShipmentDto>> Deliver(
        Guid id,
        [FromBody] DeliverShipmentDto dto,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new DeliverShipmentCommand(id, dto, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Teslimatı başarısız olarak işaretle
    /// </summary>
    [HttpPost("{id:guid}/fail")]
    public async Task<ActionResult<ShipmentDto>> FailDelivery(
        Guid id,
        [FromBody] FailDeliveryDto dto,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new FailDeliveryCommand(id, dto, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Teslimatı tekrar dene
    /// </summary>
    [HttpPost("{id:guid}/retry")]
    public async Task<ActionResult<ShipmentDto>> RetryDelivery(
        Guid id,
        [FromQuery] string? newAddress,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new RetryDeliveryCommand(id, newAddress, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }
}