using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegulatoryReporting.Application.Commands;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Application.Queries;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportDefinitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm rapor tanımlarını getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReportDefinitionDto>>> GetAll(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllReportDefinitionsQuery(activeOnly);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile rapor tanımı getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportDefinitionDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReportDefinitionByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
            return NotFound(new { error = "Rapor tanımı bulunamadı" });

        return Ok(result);
    }

    /// <summary>
    /// Kuruma göre rapor tanımlarını getirir
    /// </summary>
    [HttpGet("by-authority/{authorityId:int}")]
    public async Task<ActionResult<IReadOnlyList<ReportDefinitionDto>>> GetByAuthority(
        int authorityId,
        CancellationToken cancellationToken)
    {
        var query = new GetReportDefinitionsByAuthorityQuery(authorityId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni rapor tanımı oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReportDefinitionDto>> Create(
        [FromBody] CreateReportDefinitionDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateReportDefinitionCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}