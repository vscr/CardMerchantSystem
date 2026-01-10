using HSM.Application.Commands;
using HSM.Application.DTOs;
using HSM.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HSMController : ControllerBase
{
    private readonly IMediator _mediator;

    public HSMController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Device Management

    /// <summary>
    /// HSM cihazı ekler
    /// </summary>
    [HttpPost("devices")]
    public async Task<ActionResult<HSMDeviceDto>> CreateDevice(
        [FromBody] CreateHSMDeviceDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateHSMDeviceCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetDeviceById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// HSM cihazını ID ile getirir
    /// </summary>
    [HttpGet("devices/{id:guid}")]
    public async Task<ActionResult<HSMDeviceDto>> GetDeviceById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetHSMDeviceByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    /// <summary>
    /// Tüm HSM cihazlarını getirir
    /// </summary>
    [HttpGet("devices")]
    public async Task<ActionResult<IReadOnlyList<HSMDeviceDto>>> GetAllDevices(
        CancellationToken cancellationToken)
    {
        var query = new GetAllHSMDevicesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Key Management

    /// <summary>
    /// HSM key oluşturur
    /// </summary>
    [HttpPost("keys")]
    public async Task<ActionResult<HSMKeyDto>> GenerateKey(
        [FromBody] CreateHSMKeyDto dto,
        CancellationToken cancellationToken)
    {
        var command = new GenerateKeyCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return CreatedAtAction(nameof(GetAllKeys), result.Value);
    }

    /// <summary>
    /// Tüm HSM key'leri getirir
    /// </summary>
    [HttpGet("keys")]
    public async Task<ActionResult<IReadOnlyList<HSMKeyDto>>> GetAllKeys(
        CancellationToken cancellationToken)
    {
        var query = new GetAllHSMKeysQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region PIN Operations

    /// <summary>
    /// PIN Block oluşturur
    /// </summary>
    [HttpPost("pin/generate-block")]
    public async Task<ActionResult<GeneratePINBlockResponseDto>> GeneratePINBlock(
        [FromBody] GeneratePINBlockRequestDto dto,
        CancellationToken cancellationToken)
    {
        var command = new GeneratePINBlockCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion

    #region CVV Operations

    /// <summary>
    /// CVV doğrular
    /// </summary>
    [HttpPost("cvv/verify")]
    public async Task<ActionResult<VerifyCVVResponseDto>> VerifyCVV(
        [FromBody] VerifyCVVRequestDto dto,
        CancellationToken cancellationToken)
    {
        var command = new VerifyCVVCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion

    #region Health Check

    /// <summary>
    /// HSM sağlık kontrolü yapar
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult<HSMHealthCheckResponseDto>> HealthCheck(
        CancellationToken cancellationToken)
    {
        var command = new HealthCheckCommand();
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error, code = result.ErrorCode });

        return Ok(result.Value);
    }

    #endregion

    #region Command Logs

    /// <summary>
    /// HSM komut loglarını getirir
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<IReadOnlyList<HSMCommandLogDto>>> GetCommandLogs(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetCommandLogsQuery(startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion
}