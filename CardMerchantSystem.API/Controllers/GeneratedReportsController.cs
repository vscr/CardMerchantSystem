using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegulatoryReporting.Application.Commands;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Application.Queries;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class GeneratedReportsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public GeneratedReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gönderilmemiş raporları getirir
    /// </summary>
    [HttpGet("pending-submission")]
    public async Task<ActionResult<IReadOnlyList<GeneratedReportDto>>> GetPendingSubmission(
        CancellationToken cancellationToken)
    {
        var query = new GetPendingSubmissionReportsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Duruma göre raporları getirir
    /// </summary>
    [HttpGet("by-status/{statusId:int}")]
    public async Task<ActionResult<IReadOnlyList<GeneratedReportDto>>> GetByStatus(
        int statusId,
        CancellationToken cancellationToken)
    {
        var query = new GetGeneratedReportsByStatusQuery(statusId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ID ile rapor getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GeneratedReportDto>> GetById(
        Guid id,
        [FromQuery] bool includeSubmissions = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGeneratedReportByIdQuery(id, includeSubmissions);
        var result = await _mediator.Send(query, cancellationToken);

        return OkOrNotFound(result, "Rapor", id);
    }

    /// <summary>
    /// Rapor üretir
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedReportDto>> Generate(
        [FromBody] GenerateReportDto dto,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new GenerateReportCommand(dto, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetById), x => new { id = x.Id });
    }

    /// <summary>
    /// Raporu doğrular
    /// </summary>
    [HttpPost("{id:guid}/validate")]
    public async Task<ActionResult<GeneratedReportDto>> Validate(
        Guid id,
        [FromQuery] string validatedBy,
        CancellationToken cancellationToken)
    {
        var command = new ValidateReportCommand(id, validatedBy);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Raporu gönderir
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<GeneratedReportDto>> Submit(
        Guid id,
        [FromBody] SubmitReportDto dto,
        [FromQuery] string submittedBy,
        CancellationToken cancellationToken)
    {
        dto.GeneratedReportId = id;
        var command = new SubmitReportCommand(dto, submittedBy);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }
}