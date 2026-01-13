using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public record SetFtpDeliveryCommand(SetFtpDeliveryDto Dto) : IRequest<Result<MerchantReportConfigDto>>;