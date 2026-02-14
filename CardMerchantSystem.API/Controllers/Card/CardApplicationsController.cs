using Card.Application.Commands;
using Card.Application.DTOs;
using Card.Application.Queries;
using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.Card;

[Authorize(Policy = Policies.CardManagement)]
public class CardApplicationsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CardApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Başvuruları sayfalı listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CardApplicationDto>>> GetPaged(
        [FromQuery] CardApplicationFilterDto filter,
        CancellationToken cancellationToken)
    {
        var query = new GetCardApplicationsPagedQuery(filter);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile başvuru getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CardApplicationDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCardApplicationByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(HandleResult(result));
    }

    /// <summary>
    /// Başvuru ve durum geçmişini getirir
    /// </summary>
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<ApplicationWithHistoryDto>> GetWithHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetApplicationWithHistoryQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(HandleResult(result));
    }

    /// <summary>
    /// Duruma göre başvuruları getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<CardApplicationDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetCardApplicationsByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni kart başvurusu oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CardApplicationDto>> Create(
        [FromBody] CreateCardApplicationDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCardApplicationCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        var value = HandleResult(result);
        return CreatedResponse(nameof(GetById), new { id = value.Id }, value);
    }

    /// <summary>
    /// Başvuruyu incelemeye alır
    /// </summary>
    [HttpPost("{id:guid}/start-review")]
    public async Task<ActionResult<CardApplicationDto>> StartReview(
        Guid id,
        [FromQuery] string reviewerUsername,
        CancellationToken cancellationToken)
    {
        var command = new StartReviewCommand(id, reviewerUsername);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(HandleResult(result));
    }

    /// <summary>
    /// Başvuruyu onaylar
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromQuery] string approverUsername,
        CancellationToken cancellationToken)
    {
        var command = new ApproveCardApplicationCommand(id, approverUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Başvuru onaylandı" });
    }

    /// <summary>
    /// Başvuruyu reddeder
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RejectCardApplicationCommand(id, request.Reason, request.RejectorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Başvuru reddedildi" });
    }

    /// <summary>
    /// Kart basım talebi oluşturur
    /// </summary>
    [HttpPost("{id:guid}/request-print")]
    public async Task<IActionResult> RequestPrint(
        Guid id,
        [FromBody] RequestPrintDto request,
        CancellationToken cancellationToken)
    {
        var command = new RequestCardPrintCommand(id, request.PrintVendorId, request.BatchId, request.OperatorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Kart basım talebi oluşturuldu" });
    }

    /// <summary>
    /// Kartı basıldı olarak işaretler
    /// </summary>
    [HttpPost("{id:guid}/mark-printed")]
    public async Task<IActionResult> MarkAsPrinted(
        Guid id,
        [FromBody] MarkPrintedDto request,
        CancellationToken cancellationToken)
    {
        var command = new MarkCardAsPrintedCommand(id, request.EncryptedCardNumber, request.MaskedCardNumber, request.OperatorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Kart basıldı olarak işaretlendi" });
    }

    /// <summary>
    /// Teslimatı başlatır
    /// </summary>
    [HttpPost("{id:guid}/start-delivery")]
    public async Task<IActionResult> StartDelivery(
        Guid id,
        [FromBody] StartDeliveryDto request,
        CancellationToken cancellationToken)
    {
        var command = new StartDeliveryCommand(id, request.TrackingNumber, request.OperatorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Teslimat başlatıldı" });
    }

    /// <summary>
    /// Kartı teslim edildi olarak işaretler
    /// </summary>
    [HttpPost("{id:guid}/mark-delivered")]
    public async Task<IActionResult> MarkAsDelivered(
        Guid id,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new MarkAsDeliveredCommand(id, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Kart teslim edildi" });
    }

    /// <summary>
    /// Başvuruyu iptal eder
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CancelCardApplicationCommand(id, request.Reason, request.OperatorUsername);
        var result = await _mediator.Send(command, cancellationToken);
        HandleResult(result);
        return Ok(new { message = "Başvuru iptal edildi" });
    }
}

// Request DTOs
public record RejectRequestDto(string Reason, string RejectorUsername);
public record RequestPrintDto(int PrintVendorId, string BatchId, string OperatorUsername);
public record MarkPrintedDto(string EncryptedCardNumber, string MaskedCardNumber, string OperatorUsername);
public record StartDeliveryDto(string TrackingNumber, string OperatorUsername);
public record CancelRequestDto(string Reason, string OperatorUsername);