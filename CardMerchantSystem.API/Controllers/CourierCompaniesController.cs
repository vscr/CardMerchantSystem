using Courier.Application.Commands;
using Courier.Application.DTOs;
using Courier.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class CourierCompaniesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourierCompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm kurye firmalarını getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourierCompanyDto>>> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCourierCompaniesQuery(activeOnly);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile kurye firması getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourierCompanyDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCourierCompanyByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return OkOrNotFound(result, "Kurye firması", id);
    }

    /// <summary>
    /// Yeni kurye firması oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CourierCompanyDto>> Create(
        [FromBody] CreateCourierCompanyDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCourierCompanyCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetById), x => new { id = x.Id });
    }
}