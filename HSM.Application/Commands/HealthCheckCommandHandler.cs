using HSM.Application.DTOs;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public class HealthCheckCommandHandler
    : IRequestHandler<HealthCheckCommand, Result<HSMHealthCheckResponseDto>>
{
    private readonly IHSMService _hsmService;
    private readonly IHSMDeviceRepository _deviceRepository;

    public HealthCheckCommandHandler(
        IHSMService hsmService,
        IHSMDeviceRepository deviceRepository)
    {
        _hsmService = hsmService;
        _deviceRepository = deviceRepository;
    }

    public async Task<Result<HSMHealthCheckResponseDto>> Handle(
        HealthCheckCommand request,
        CancellationToken cancellationToken)
    {
        // Primary device bul
        var device = await _deviceRepository.GetPrimaryDeviceAsync(cancellationToken);
        if (device == null)
            return Result.Failure<HSMHealthCheckResponseDto>("HSM cihazı bulunamadı");

        try
        {
            var result = await _hsmService.HealthCheckAsync(cancellationToken);

            if (result.IsFailure)
            {
                device.UpdateStatus(HSMConnectionStatus.Error);
                await _deviceRepository.UpdateAsync(device, cancellationToken);
                await _deviceRepository.SaveChangesAsync(cancellationToken);

                return new HSMHealthCheckResponseDto
                {
                    IsHealthy = false,
                    DeviceName = device.DeviceName,
                    Status = "Error",
                    ErrorMessage = result.Error
                };
            }

            var response = result.Value!;

            device.UpdateStatus(response.IsHealthy ? HSMConnectionStatus.Connected : HSMConnectionStatus.Error);
            await _deviceRepository.UpdateAsync(device, cancellationToken);
            await _deviceRepository.SaveChangesAsync(cancellationToken);

            return new HSMHealthCheckResponseDto
            {
                IsHealthy = response.IsHealthy,
                DeviceName = device.DeviceName,
                Status = response.Status,
                ResponseTimeMs = response.ResponseTimeMs,
                FirmwareVersion = response.FirmwareVersion
            };
        }
        catch (Exception ex)
        {
            device.UpdateStatus(HSMConnectionStatus.Error);
            await _deviceRepository.UpdateAsync(device, cancellationToken);
            await _deviceRepository.SaveChangesAsync(cancellationToken);

            return new HSMHealthCheckResponseDto
            {
                IsHealthy = false,
                DeviceName = device.DeviceName,
                Status = "Error",
                ErrorMessage = ex.Message
            };
        }
    }
}