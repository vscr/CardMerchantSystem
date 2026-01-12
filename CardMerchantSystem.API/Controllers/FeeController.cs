using Fee.Application.Commands;
using Fee.Application.DTOs;
using Fee.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Tariff Management

    /// <summary>
    /// Tarife oluşturur
    /// </summary>
    [HttpPost("tariffs")]
    public async Task<ActionResult<TariffDto>> CreateTariff(
        [FromBody] CreateTariffDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateTariffCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetTariffById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Tarifeye kural ekler
    /// </summary>
    [HttpPost("tariffs/rules")]
    public async Task<ActionResult<TariffDto>> AddTariffRule(
        [FromBody] AddTariffRuleDto dto,
        CancellationToken cancellationToken)
    {
        var command = new AddTariffRuleCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tarifeyi aktif eder
    /// </summary>
    [HttpPut("tariffs/{id:guid}/activate")]
    public async Task<ActionResult<TariffDto>> ActivateTariff(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ActivateTariffCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tarife detayını getirir
    /// </summary>
    [HttpGet("tariffs/{id:guid}")]
    public async Task<ActionResult<TariffDto>> GetTariffById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTariffByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tüm tarifeleri listeler
    /// </summary>
    [HttpGet("tariffs")]
    public async Task<ActionResult<IReadOnlyList<TariffDto>>> GetAllTariffs(
        CancellationToken cancellationToken)
    {
        var query = new GetAllTariffsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Merchant Tariff Assignment

    /// <summary>
    /// Üye işyerine tarife atar
    /// </summary>
    [HttpPost("merchant-tariffs")]
    public async Task<ActionResult<MerchantTariffDto>> AssignMerchantTariff(
        [FromBody] AssignMerchantTariffDto dto,
        CancellationToken cancellationToken)
    {
        var command = new AssignMerchantTariffCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetMerchantTariffs), new { merchantId = dto.MerchantId }, result.Value);
    }

    /// <summary>
    /// Üye işyerinin tarifelerini getirir
    /// </summary>
    [HttpGet("merchant-tariffs/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<MerchantTariffDto>>> GetMerchantTariffs(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantTariffsQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Commission Calculation

    /// <summary>
    /// Komisyon hesaplar
    /// </summary>
    [HttpPost("calculate-commission")]
    public async Task<ActionResult<CalculateCommissionResponseDto>> CalculateCommission(
        [FromBody] CalculateCommissionRequestDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CalculateCommissionCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Üye işyeri komisyon özetini getirir
    /// </summary>
    [HttpGet("commission-summary/{merchantId}")]
    public async Task<ActionResult<MerchantCommissionSummaryDto>> GetMerchantCommissionSummary(
        string merchantId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantCommissionSummaryQuery(merchantId, startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion

    #region Membership Fee

    /// <summary>
    /// Aidat tanımı oluşturur
    /// </summary>
    [HttpPost("membership-fees")]
    public async Task<ActionResult<MembershipFeeDto>> CreateMembershipFee(
        [FromBody] CreateMembershipFeeDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateMembershipFeeCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetAllMembershipFees), result.Value);
    }

    /// <summary>
    /// Tüm aidat tanımlarını listeler
    /// </summary>
    [HttpGet("membership-fees")]
    public async Task<ActionResult<IReadOnlyList<MembershipFeeDto>>> GetAllMembershipFees(
        CancellationToken cancellationToken)
    {
        var query = new GetAllMembershipFeesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Fee Accruals

    /// <summary>
    /// Tahakkuk oluşturur
    /// </summary>
    [HttpPost("accruals")]
    public async Task<ActionResult<FeeAccrualDto>> CreateFeeAccrual(
        [FromBody] CreateFeeAccrualDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateFeeAccrualCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetMerchantAccruals), new { merchantId = dto.MerchantId }, result.Value);
    }

    /// <summary>
    /// Ödeme kaydeder
    /// </summary>
    [HttpPost("accruals/payment")]
    public async Task<ActionResult<FeeAccrualDto>> RecordPayment(
        [FromBody] RecordPaymentDto dto,
        CancellationToken cancellationToken)
    {
        var command = new RecordPaymentCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Üye işyeri tahakkuklarını getirir
    /// </summary>
    [HttpGet("accruals/merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<FeeAccrualDto>>> GetMerchantAccruals(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetFeeAccrualsByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Vadesi geçmiş tahakkukları getirir
    /// </summary>
    [HttpGet("accruals/overdue")]
    public async Task<ActionResult<IReadOnlyList<FeeAccrualDto>>> GetOverdueAccruals(
        CancellationToken cancellationToken)
    {
        var query = new GetOverdueAccrualsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion
}