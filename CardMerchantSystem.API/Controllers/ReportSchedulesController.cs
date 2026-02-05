using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegulatoryReporting.Application.Commands;
using RegulatoryReporting.Application.DTOs;

namespace CardMerchantSystem.API.Controllers;

[Authorize]
public class ReportSchedulesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ReportSchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni rapor zamanlaması oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReportScheduleDto>> Create(
        [FromBody] CreateReportScheduleDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateReportScheduleCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        return ToActionResult(result);
    }
}