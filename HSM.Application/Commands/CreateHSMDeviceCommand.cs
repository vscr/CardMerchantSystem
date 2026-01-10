using HSM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public record CreateHSMDeviceCommand(CreateHSMDeviceDto Dto) : IRequest<Result<HSMDeviceDto>>;