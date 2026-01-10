using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;
using System.Diagnostics;

namespace HSM.Application.Commands;

public class VerifyCVVCommandHandler
    : IRequestHandler<VerifyCVVCommand, Result<VerifyCVVResponseDto>>
{
    private readonly IHSMService _hsmService;
    private readonly IHSMKeyRepository _keyRepository;
    private readonly IHSMDeviceRepository _deviceRepository;
    private readonly IHSMCommandLogRepository _logRepository;

    public VerifyCVVCommandHandler(
        IHSMService hsmService,
        IHSMKeyRepository keyRepository,
        IHSMDeviceRepository deviceRepository,
        IHSMCommandLogRepository logRepository)
    {
        _hsmService = hsmService;
        _keyRepository = keyRepository;
        _deviceRepository = deviceRepository;
        _logRepository = logRepository;
    }

    public async Task<Result<VerifyCVVResponseDto>> Handle(
        VerifyCVVCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var stopwatch = Stopwatch.StartNew();

        // CVK key'i bul
        var cvkKeys = await _keyRepository.GetByTypeAsync(HSMKeyType.CVK, cancellationToken);
        var cvkKey = cvkKeys.FirstOrDefault(k => k.IsActive && !k.IsExpired);

        if (cvkKey == null)
            return Result.Failure<VerifyCVVResponseDto>("Aktif CVK key bulunamadı");

        // Primary device bul
        var device = await _deviceRepository.GetPrimaryDeviceAsync(cancellationToken);
        if (device == null)
            return Result.Failure<VerifyCVVResponseDto>("Aktif HSM cihazı bulunamadı");

        // Kart numarasını maskele
        var cardNumberClean = dto.CardNumber.Replace(" ", "");
        var maskedCard = $"{cardNumberClean.Substring(0, 6)}******{cardNumberClean.Substring(12)}";

        // Log kaydı oluştur
        var log = HSMCommandLog.Create(
            device.Id,
            HSMCommandType.VerifyCVV,
            $"CardNumber:{maskedCard},CVV:***",
            Guid.NewGuid().ToString(),
            maskedCard);

        try
        {
            // HSM'e gönder
            var hsmRequest = new VerifyCVVRequest
            {
                CardNumber = dto.CardNumber,
                ExpiryDate = dto.ExpiryDate,
                ServiceCode = dto.ServiceCode,
                CVV = dto.CVV,
                CVKIndex = cvkKey.KeyIndex
            };

            var result = await _hsmService.VerifyCVVAsync(hsmRequest, cancellationToken);

            stopwatch.Stop();

            if (result.IsFailure)
            {
                log.SetResponse("", "99", false, (int)stopwatch.ElapsedMilliseconds, result.Error);
                await _logRepository.AddAsync(log, cancellationToken);
                await _logRepository.SaveChangesAsync(cancellationToken);

                return new VerifyCVVResponseDto
                {
                    IsSuccess = false,
                    IsValid = false,
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    ErrorMessage = result.Error
                };
            }

            var response = result.Value!;
            log.SetResponse(response.ResponseCode, response.ResponseCode, response.IsValid, (int)stopwatch.ElapsedMilliseconds);
            await _logRepository.AddAsync(log, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            device.RecordSuccessfulCommand();
            await _deviceRepository.UpdateAsync(device, cancellationToken);
            await _deviceRepository.SaveChangesAsync(cancellationToken);

            return new VerifyCVVResponseDto
            {
                IsSuccess = true,
                IsValid = response.IsValid,
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            log.SetResponse("", "99", false, (int)stopwatch.ElapsedMilliseconds, ex.Message);
            await _logRepository.AddAsync(log, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return new VerifyCVVResponseDto
            {
                IsSuccess = false,
                IsValid = false,
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };
        }
    }
}