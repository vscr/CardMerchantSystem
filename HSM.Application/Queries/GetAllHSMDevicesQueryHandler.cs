using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Repositories;
using MediatR;

namespace HSM.Application.Queries;

public class GetAllHSMDevicesQueryHandler
    : IRequestHandler<GetAllHSMDevicesQuery, IReadOnlyList<HSMDeviceDto>>
{
    private readonly IHSMDeviceRepository _repository;

    public GetAllHSMDevicesQueryHandler(IHSMDeviceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<HSMDeviceDto>> Handle(
        GetAllHSMDevicesQuery request,
        CancellationToken cancellationToken)
    {
        var devices = await _repository.GetAllActiveAsync(cancellationToken);

        return devices.Select(MapToDto).ToList();
    }

    private static HSMDeviceDto MapToDto(HSMDevice device)
    {
        return new HSMDeviceDto
        {
            Id = device.Id,
            DeviceName = device.DeviceName,
            DeviceType = device.DeviceType.Name,
            DeviceTypeDisplayName = device.DeviceType.DisplayName,
            IpAddress = device.IpAddress,
            Port = device.Port,
            SecondaryIpAddress = device.SecondaryIpAddress,
            SecondaryPort = device.SecondaryPort,
            Status = device.Status.Name,
            StatusDisplayName = device.Status.DisplayName,
            IsActive = device.IsActive,
            IsPrimary = device.IsPrimary,
            TimeoutMs = device.TimeoutMs,
            RetryCount = device.RetryCount,
            LastHealthCheck = device.LastHealthCheck,
            LastSuccessfulCommand = device.LastSuccessfulCommand,
            CreatedAt = device.CreatedAt
        };
    }
}