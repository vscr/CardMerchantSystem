using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transaction.Application.Commands;
using Transaction.Application.DTOs;
using Transaction.Application.Queries;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class TransactionsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// İşlemleri sayfalı listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<TransactionDto>>> GetPaged(
        [FromQuery] TransactionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionsPagedQuery(filter);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// İşlem gerçekleştirir (Satış, İade, İptal vb.)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionResultDto>> Process(
        [FromBody] CreateTransactionDto dto,
        CancellationToken cancellationToken)
    {
        var command = new ProcessTransactionCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(HandleResult(result));
    }

    /// <summary>
    /// İade işlemi yapar
    /// </summary>
    [HttpPost("{originalTransactionId:guid}/refund")]
    public async Task<ActionResult<TransactionResultDto>> Refund(
        Guid originalTransactionId,
        [FromQuery] decimal amount,
        [FromQuery] string operatorUsername,
        CancellationToken cancellationToken)
    {
        var command = new RefundTransactionCommand(originalTransactionId, amount, operatorUsername);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(HandleResult(result));
    }

    /// <summary>
    /// ID ile işlem getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(HandleResult(result));
    }

    /// <summary>
    /// Üye işyerine göre işlemleri getirir
    /// </summary>
    [HttpGet("by-merchant/{merchantId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetByMerchant(
        Guid merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionsByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Kart numarasına göre işlemleri getirir
    /// </summary>
    [HttpGet("by-card/{cardNumberMasked}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetByCard(
        string cardNumberMasked,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionsByCardQuery(cardNumberMasked);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Tarih aralığına göre işlemleri getirir
    /// </summary>
    [HttpGet("by-date")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionsByDateRangeQuery(startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// İşlem istatistiklerini getirir (Dashboard için)
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<TransactionStatsDto>> GetStats(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionStatsQuery(startDate, endDate, merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Kart limit bilgisini getirir
    /// </summary>
    [HttpGet("limits/{cardNumberMasked}")]
    public async Task<ActionResult<LimitInfoDto>> GetCardLimit(
        string cardNumberMasked,
        CancellationToken cancellationToken)
    {
        var query = new GetCardLimitQuery(cardNumberMasked);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(HandleResult(result));
    }

    /// <summary>
    /// Günsonu takas işlemi yapar
    /// </summary>
    [HttpPost("settle")]
    public async Task<ActionResult<SettlementResultDto>> Settle(
        [FromQuery] string batchNumber,
        CancellationToken cancellationToken)
    {
        var command = new SettleTransactionsCommand(batchNumber);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(HandleResult(result));
    }
}