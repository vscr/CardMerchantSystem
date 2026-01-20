using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOrder.Application.Commands;
using WorkOrder.Application.DTOs;
using WorkOrder.Application.Queries;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrdersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("open")]
    public async Task<ActionResult<IReadOnlyList<WorkOrderItemDto>>> GetOpen(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOpenWorkOrdersQuery(), cancellationToken));

    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<WorkOrderItemDto>>> GetOverdue(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOverdueWorkOrdersQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkOrderItemDto>> GetById(Guid id, [FromQuery] bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetWorkOrderByIdQuery(id, includeDetails), cancellationToken);
        return result is null ? NotFound(new { error = "İş emri bulunamadı" }) : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrderItemDto>> Create([FromBody] CreateWorkOrderItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateWorkOrderItemCommand(dto), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<ActionResult<WorkOrderItemDto>> Assign(Guid id, [FromBody] AssignWorkOrderDto dto, [FromQuery] string operatorUsername, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AssignWorkOrderCommand(id, dto, operatorUsername), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<WorkOrderItemDto>> Complete(Guid id, [FromQuery] string resolution, [FromQuery] string completedBy, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteWorkOrderCommand(id, resolution, completedBy), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<WorkOrderNoteDto>> AddNote(Guid id, [FromBody] AddNoteDto dto, [FromQuery] string createdBy, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddWorkOrderNoteCommand(id, dto, createdBy), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/process-approval")]
    public async Task<ActionResult<WorkOrderItemDto>> ProcessApproval(Guid id, [FromBody] ProcessApprovalDto dto, [FromQuery] string operatorUsername, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ProcessApprovalCommand(id, dto, operatorUsername), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : Ok(result.Value);
    }
}