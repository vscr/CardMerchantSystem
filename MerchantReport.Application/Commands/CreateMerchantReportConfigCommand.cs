using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public record CreateMerchantReportConfigCommand(CreateMerchantReportConfigDto Dto) : IRequest<Result<MerchantReportConfigDto>>;