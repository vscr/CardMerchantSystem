using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public class CreateHSMDeviceCommandHandler
    : IRequestHandler<CreateHSMDeviceCommand, Result<HSMDeviceDto>>
{
    private readonly IHSMDeviceRepository _repository;

    public CreateHSMDeviceCommandHandler(IHSMDeviceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<HSMDeviceDto>> Handle(
        CreateHSMDeviceCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı isimde cihaz var mı kontrol et
        var existing = await _repository.GetByNameAsync(dto.DeviceName, cancellationToken);
        if (existing != null)
            return Result.Failure<HSMDeviceDto>("Bu isimde bir HSM cihazı zaten mevcut");

        // Device type bul
        var deviceType = HSMDeviceType.FromId<HSMDeviceType>(dto.DeviceTypeId);
        if (deviceType == null)
            return Result.Failure<HSMDeviceDto>("Geçersiz cihaz tipi");

        // Device oluştur
        var deviceResult = HSMDevice.Create(
            dto.DeviceName,
            deviceType,
            dto.IpAddress,
            dto.Port,
            dto.IsPrimary,
            dto.TimeoutMs,
            dto.RetryCount);

        if (deviceResult.IsFailure)
            return Result.Failure<HSMDeviceDto>(deviceResult.Error!);

        var device = deviceResult.Value!;

        // Secondary connection varsa ekle
        if (!string.IsNullOrEmpty(dto.SecondaryIpAddress) && dto.SecondaryPort.HasValue)
        {
            device.SetSecondaryConnection(dto.SecondaryIpAddress, dto.SecondaryPort.Value);
        }

        await _repository.AddAsync(device, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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