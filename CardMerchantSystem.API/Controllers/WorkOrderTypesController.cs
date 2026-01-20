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
public class WorkOrderTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrderTypesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkOrderTypeDto>>> GetAll([FromQuery] bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllWorkOrderTypesQuery(activeOnly), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkOrderTypeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderTypeByIdQuery(id), cancellationToken);
        return result is null ? NotFound(new { error = "İş emri tipi bulunamadı" }) : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrderTypeDto>> Create([FromBody] CreateWorkOrderTypeDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateWorkOrderTypeCommand(dto), cancellationToken);
        return result.IsFailure ? BadRequest(new { error = result.Error }) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}