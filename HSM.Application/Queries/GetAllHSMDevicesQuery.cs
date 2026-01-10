using HSM.Application.DTOs;
using MediatR;

namespace HSM.Application.Queries;

public record GetAllHSMDevicesQuery() : IRequest<IReadOnlyList<HSMDeviceDto>>;