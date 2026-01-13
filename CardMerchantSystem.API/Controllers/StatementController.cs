using Statement.Application.Commands;
using Statement.Application.DTOs;
using Statement.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatementController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Statement Management

    /// <summary>
    /// Ekstre oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CardStatementDto>> CreateStatement(
        [FromBody] CreateStatementDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateStatementCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetStatementById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Ekstreye kalem ekler
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<CardStatementDto>> AddStatementItem(
        [FromBody] AddStatementItemDto dto,
        CancellationToken cancellationToken)
    {
        var command = new AddStatementItemCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Ekstreyi tamamlar (oluşturuldu olarak işaretler)
    /// </summary>
    [HttpPut("{id:guid}/generate")]
    public async Task<ActionResult<CardStatementDto>> GenerateStatement(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new GenerateStatementCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Ekstre detayını getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CardStatementDto>> GetStatementById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetStatementByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Karta ait ekstreleri listeler
    /// </summary>
    [HttpGet("card/{cardNumber}")]
    public async Task<ActionResult<IReadOnlyList<CardStatementSummaryDto>>> GetStatementsByCardNumber(
        string cardNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetStatementsByCardNumberQuery(cardNumber);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Vadesi geçmiş ekstreleri listeler
    /// </summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<CardStatementSummaryDto>>> GetOverdueStatements(
        CancellationToken cancellationToken)
    {
        var query = new GetOverdueStatementsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Payment

    /// <summary>
    /// Ekstre ödemesi kaydeder
    /// </summary>
    [HttpPost("payment")]
    public async Task<ActionResult<StatementPaymentResultDto>> RecordPayment(
        [FromBody] RecordStatementPaymentDto dto,
        CancellationToken cancellationToken)
    {
        var command = new RecordStatementPaymentCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion

    #region PDF

    /// <summary>
    /// Ekstre PDF'i oluşturur ve indirir
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new GeneratePdfCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return File(result.Value!, "application/pdf", $"Statement_{id}.pdf");
    }

    #endregion

    #region Period Config

    /// <summary>
    /// Kesim ayarı oluşturur
    /// </summary>
    [HttpPost("period-config")]
    public async Task<ActionResult<StatementPeriodConfigDto>> CreatePeriodConfig(
        [FromBody] CreateStatementPeriodConfigDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateStatementPeriodConfigCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetPeriodConfig), new { cardNumber = dto.CardNumber }, result.Value);
    }

    /// <summary>
    /// Kart kesim ayarını getirir
    /// </summary>
    [HttpGet("period-config/{cardNumber}")]
    public async Task<ActionResult<StatementPeriodConfigDto>> GetPeriodConfig(
        string cardNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetStatementPeriodConfigQuery(cardNumber);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion
}