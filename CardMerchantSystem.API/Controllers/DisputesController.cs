using CardMerchantSystem.Shared.Kernel;
using Dispute.Application.Commands;
using Dispute.Application.DTOs;
using Dispute.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisputesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DisputesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// İtirazları sayfalı listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<DisputeDto>>> GetPaged(
        [FromQuery] DisputeFilterDto filter,
        CancellationToken cancellationToken)
    {
        var query = new GetDisputesPagedQuery(filter);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni itiraz oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DisputeDto>> Create(
        [FromBody] CreateDisputeDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateDisputeCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// ID ile itiraz getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DisputeDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetDisputeByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// İtiraz detaylarını getirir (dökümanlar ve notlarla birlikte)
    /// </summary>
    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<DisputeDetailDto>> GetDetails(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetDisputeDetailQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Duruma göre itirazları getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<DisputeDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetDisputesByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Süresi geçmiş itirazları getirir
    /// </summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<DisputeDto>>> GetOverdue(
        CancellationToken cancellationToken)
    {
        var query = new GetOverdueDisputesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// İtirazı incelemeye alır
    /// </summary>
    [HttpPost("{id:guid}/start-review")]
    public async Task<IActionResult> StartReview(
        Guid id,
        [FromQuery] string assignedTo,
        CancellationToken cancellationToken)
    {
        var command = new StartReviewCommand(id, assignedTo);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "İtiraz incelemeye alındı" });
    }

    /// <summary>
    /// İtirazı çözer
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveDisputeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResolveDisputeCommand(
            id,
            request.InFavorOfCustomer,
            request.RefundAmount,
            request.Resolution,
            request.OperatorUsername);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "İtiraz çözüldü" });
    }

    /// <summary>
    /// İtirazı bankaya yönlendirir
    /// </summary>
    [HttpPost("{id:guid}/escalate")]
    public async Task<IActionResult> Escalate(
        Guid id,
        [FromQuery] string escalationReason,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new EscalateDisputeCommand(id, escalationReason, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "İtiraz bankaya yönlendirildi" });
    }

    /// <summary>
    /// İtiraza not ekler
    /// </summary>
    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(
        Guid id,
        [FromBody] AddNoteRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddDisputeNoteCommand(id, request.Note, request.Username, request.IsInternal);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(new { message = "Not eklendi" });
    }
}

// Request DTOs
public class ResolveDisputeRequest
{
    public bool InFavorOfCustomer { get; set; }
    public decimal? RefundAmount { get; set; }
    public string Resolution { get; set; } = null!;
    public string OperatorUsername { get; set; } = null!;
}

public class AddNoteRequest
{
    public string Note { get; set; } = null!;
    public string Username { get; set; } = null!;
    public bool IsInternal { get; set; } = true;
}