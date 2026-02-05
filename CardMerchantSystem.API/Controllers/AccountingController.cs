using Accounting.Application.Commands;
using Accounting.Application.DTOs;
using Accounting.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class AccountingController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AccountingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Chart of Accounts

    /// <summary>
    /// Hesap oluşturur
    /// </summary>
    [HttpPost("accounts")]
    public async Task<ActionResult<ChartOfAccountDto>> CreateAccount(
        [FromBody] CreateChartOfAccountDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateChartOfAccountCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Hesap planını listeler
    /// </summary>
    [HttpGet("accounts")]
    public async Task<ActionResult<IReadOnlyList<ChartOfAccountDto>>> GetAccounts(
        CancellationToken cancellationToken)
    {
        var query = new GetChartOfAccountsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Accounting Periods

    /// <summary>
    /// Muhasebe dönemi oluşturur
    /// </summary>
    [HttpPost("periods")]
    public async Task<ActionResult<AccountingPeriodDto>> CreatePeriod(
        [FromBody] CreateAccountingPeriodDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountingPeriodCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Dönemleri listeler
    /// </summary>
    [HttpGet("periods")]
    public async Task<ActionResult<IReadOnlyList<AccountingPeriodDto>>> GetPeriods(
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountingPeriodsQuery(year);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Dönemi kapatır
    /// </summary>
    [HttpPut("periods/{periodCode}/close")]
    public async Task<ActionResult<AccountingPeriodDto>> ClosePeriod(
        string periodCode,
        CancellationToken cancellationToken)
    {
        var command = new ClosePeriodCommand(periodCode, CurrentUsername ?? "System");
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    #endregion

    #region Journal Entries

    /// <summary>
    /// Muhasebe fişi oluşturur
    /// </summary>
    [HttpPost("journal-entries")]
    public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry(
        [FromBody] CreateJournalEntryDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateJournalEntryCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetJournalEntryById), x => new { id = x.Id });
    }

    /// <summary>
    /// Muhasebe fişini onaylar
    /// </summary>
    [HttpPut("journal-entries/{id:guid}/post")]
    public async Task<ActionResult<JournalEntryDto>> PostJournalEntry(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new PostJournalEntryCommand(id, CurrentUsername ?? "System");
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Muhasebe fişini iptal eder (ters kayıt)
    /// </summary>
    [HttpPut("journal-entries/{id:guid}/reverse")]
    public async Task<ActionResult<JournalEntryDto>> ReverseJournalEntry(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ReverseJournalEntryCommand(id, CurrentUsername ?? "System");
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Muhasebe fişi detayını getirir
    /// </summary>
    [HttpGet("journal-entries/{id:guid}")]
    public async Task<ActionResult<JournalEntryDto>> GetJournalEntryById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetJournalEntryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Döneme ait muhasebe fişlerini listeler
    /// </summary>
    [HttpGet("journal-entries/period/{periodCode}")]
    public async Task<ActionResult<IReadOnlyList<JournalEntrySummaryDto>>> GetJournalEntriesByPeriod(
        string periodCode,
        CancellationToken cancellationToken)
    {
        var query = new GetJournalEntriesByPeriodQuery(periodCode);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Trial Balance

    /// <summary>
    /// Mizan raporu getirir
    /// </summary>
    [HttpGet("trial-balance/{periodCode}")]
    public async Task<ActionResult<TrialBalanceDto>> GetTrialBalance(
        string periodCode,
        CancellationToken cancellationToken)
    {
        var query = new GetTrialBalanceQuery(periodCode);
        var result = await _mediator.Send(query, cancellationToken);

        return ToActionResult(result);
    }

    #endregion
}