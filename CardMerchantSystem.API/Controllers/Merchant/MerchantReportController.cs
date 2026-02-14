using MerchantReport.Application.Commands;
using MerchantReport.Application.DTOs;
using MerchantReport.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.Merchant;

[Authorize]
public class MerchantReportController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public MerchantReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Report Config

    /// <summary>
    /// Rapor ayarı oluşturur
    /// </summary>
    [HttpPost("configs")]
    public async Task<ActionResult<MerchantReportConfigDto>> CreateConfig(
        [FromBody] CreateMerchantReportConfigDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateMerchantReportConfigCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetConfigsByMerchant), x => new { merchantId = dto.MerchantId });
    }

    /// <summary>
    /// Email dağıtım ayarı günceller
    /// </summary>
    [HttpPut("configs/email")]
    public async Task<ActionResult<MerchantReportConfigDto>> SetEmailDelivery(
        [FromBody] SetEmailDeliveryDto dto,
        CancellationToken cancellationToken)
    {
        var command = new SetEmailDeliveryCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// FTP dağıtım ayarı günceller
    /// </summary>
    [HttpPut("configs/ftp")]
    public async Task<ActionResult<MerchantReportConfigDto>> SetFtpDelivery(
        [FromBody] SetFtpDeliveryDto dto,
        CancellationToken cancellationToken)
    {
        var command = new SetFtpDeliveryCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Üye işyeri rapor ayarlarını listeler
    /// </summary>
    [HttpGet("configs/merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<MerchantReportConfigDto>>> GetConfigsByMerchant(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantReportConfigsQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Report Requests

    /// <summary>
    /// Rapor talebi oluşturur
    /// </summary>
    [HttpPost("requests")]
    public async Task<ActionResult<ReportRequestDto>> CreateRequest(
        [FromBody] CreateReportRequestDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateReportRequestCommand(dto, User.Identity?.Name);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedOrBadRequest(result, nameof(GetRequestById), x => new { id = x.Id });
    }

    /// <summary>
    /// Rapor üretir
    /// </summary>
    [HttpPost("requests/{id:guid}/generate")]
    public async Task<ActionResult<ReportRequestDto>> GenerateReport(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new GenerateReportCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Raporu teslim eder
    /// </summary>
    [HttpPost("requests/{id:guid}/deliver")]
    public async Task<ActionResult<ReportRequestDto>> DeliverReport(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeliverReportCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Rapor talebi detayını getirir
    /// </summary>
    [HttpGet("requests/{id:guid}")]
    public async Task<ActionResult<ReportRequestDto>> GetRequestById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReportRequestByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new ApiErrorResponse(result.Error!, result.ErrorCode));

        return Ok(result.Value);
    }

    /// <summary>
    /// Üye işyeri rapor taleplerini listeler
    /// </summary>
    [HttpGet("requests/merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<ReportRequestSummaryDto>>> GetRequestsByMerchant(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetReportRequestsByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Rapor dosyasını indirir
    /// </summary>
    [HttpGet("requests/{id:guid}/download")]
    public async Task<IActionResult> DownloadReport(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReportRequestByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new ApiErrorResponse(result.Error!, result.ErrorCode));

        var request = result.Value!;

        if (string.IsNullOrEmpty(request.FileName))
            return BadRequest(new ApiErrorResponse("Rapor henüz oluşturulmamış", "REPORT_NOT_GENERATED"));

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "MerchantReports", request.FileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new ApiErrorResponse("Rapor dosyası bulunamadı", "FILE_NOT_FOUND"));

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
        var contentType = request.ReportFormat switch
        {
            "PDF" => "application/pdf",
            "Excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "CSV" => "text/csv",
            _ => "application/octet-stream"
        };

        return File(fileBytes, contentType, request.FileName);
    }

    #endregion

    #region Statements

    /// <summary>
    /// Ekstre detayını getirir
    /// </summary>
    [HttpGet("statements/{id:guid}")]
    public async Task<ActionResult<MerchantStatementDto>> GetStatementById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantStatementByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new ApiErrorResponse(result.Error!, result.ErrorCode));

        return Ok(result.Value);
    }

    /// <summary>
    /// Üye işyeri ekstrelerini listeler
    /// </summary>
    [HttpGet("statements/merchant/{merchantId}")]
    public async Task<ActionResult<IReadOnlyList<MerchantStatementSummaryDto>>> GetStatementsByMerchant(
        string merchantId,
        CancellationToken cancellationToken)
    {
        var query = new GetMerchantStatementsByMerchantQuery(merchantId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    #endregion
}