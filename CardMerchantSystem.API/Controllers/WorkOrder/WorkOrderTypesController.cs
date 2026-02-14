using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOrder.Application.Commands;
using WorkOrder.Application.DTOs;
using WorkOrder.Application.Queries;

namespace CardMerchantSystem.API.Controllers.WorkOrder;

[Authorize]
public class WorkOrderTypesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrderTypesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkOrderTypeDto>>> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllWorkOrderTypesQuery(activeOnly), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkOrderTypeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderTypeByIdQuery(id), cancellationToken);
        return Ok(HandleNotFound(result, "İş emri tipi", id));
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrderTypeDto>> Create(
        [FromBody] CreateWorkOrderTypeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateWorkOrderTypeCommand(dto), cancellationToken);
        var value = HandleResult(result);
        return CreatedResponse(nameof(GetById), new { id = value.Id }, value);
    }
}