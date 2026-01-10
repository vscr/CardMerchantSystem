using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Queries;

public class GetHSMDeviceByIdQueryHandler
    : IRequestHandler<GetHSMDeviceByIdQuery, Result<HSMDeviceDto>>
{
    private readonly IHSMDeviceRepository _repository;

    public GetHSMDeviceByIdQueryHandler(IHSMDeviceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<HSMDeviceDto>> Handle(
        GetHSMDeviceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var device = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (device == null)
            return Result.Failure<HSMDeviceDto>("HSM cihazı bulunamadı", ErrorCodes.NotFound);

        return MapToDto(device);
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