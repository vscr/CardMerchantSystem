using EarlyBlockResolution.Application.Commands;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class BlockRulesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BlockRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm bloke kurallarını getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BlockRuleDto>>> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBlockRulesQuery(activeOnly);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile bloke kuralı getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BlockRuleDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetBlockRuleByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return OkOrNotFound(result, "Bloke kuralı", id);
    }

    /// <summary>
    /// Yeni bloke kuralı oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BlockRuleDto>> Create(
        [FromBody] CreateBlockRuleDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateBlockRuleCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetById), x => new { id = x.Id });
    }
}