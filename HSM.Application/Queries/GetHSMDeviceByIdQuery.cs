using HSM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Queries;

public record GetHSMDeviceByIdQuery(Guid Id) : IRequest<Result<HSMDeviceDto>>;