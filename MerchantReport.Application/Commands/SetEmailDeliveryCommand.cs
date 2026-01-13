using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public record SetEmailDeliveryCommand(SetEmailDeliveryDto Dto) : IRequest<Result<MerchantReportConfigDto>>;