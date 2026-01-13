using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public record GenerateReportCommand(Guid ReportRequestId) : IRequest<Result<ReportRequestDto>>;