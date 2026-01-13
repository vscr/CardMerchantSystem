using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Queries;

public record GetReportRequestByIdQuery(Guid Id) : IRequest<Result<ReportRequestDto>>;