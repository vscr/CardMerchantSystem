using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public record CreateReportRequestCommand(CreateReportRequestDto Dto, string? RequestedBy = null) : IRequest<Result<ReportRequestDto>>;