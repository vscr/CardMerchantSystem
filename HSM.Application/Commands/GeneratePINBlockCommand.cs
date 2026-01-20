using CardMerchantSystem.Shared.Kernel;
using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Domain.Services;
using MediatR;
using System.Diagnostics;

namespace HSM.Application.Commands;

public record GeneratePINBlockCommand(GeneratePINBlockRequestDto Request) : IRequest<Result<GeneratePINBlockResponseDto>>;
public class GeneratePINBlockCommandHandler
    : IRequestHandler<GeneratePINBlockCommand, Result<GeneratePINBlockResponseDto>>
{
    private readonly IHSMService _hsmService;
    private readonly IHSMKeyRepository _keyRepository;
    private readonly IHSMDeviceRepository _deviceRepository;
    private readonly IHSMCommandLogRepository _logRepository;

    public GeneratePINBlockCommandHandler(
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

    public async Task<Result<GeneratePINBlockResponseDto>> Handle(
        GeneratePINBlockCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var stopwatch = Stopwatch.StartNew();

        // ZPK key'i bul
        var zpkKeys = await _keyRepository.GetByTypeAsync(HSMKeyType.ZPK, cancellationToken);
        var zpkKey = zpkKeys.FirstOrDefault(k => k.IsActive && !k.IsExpired);

        if (zpkKey == null)
            return Result.Failure<GeneratePINBlockResponseDto>("Aktif ZPK key bulunamadı");

        // Primary device bul
        var device = await _deviceRepository.GetPrimaryDeviceAsync(cancellationToken);
        if (device == null)
            return Result.Failure<GeneratePINBlockResponseDto>("Aktif HSM cihazı bulunamadı");

        // Kart numarasını maskele
        var cardNumberClean = dto.CardNumber.Replace(" ", "");
        var maskedCard = $"{cardNumberClean.Substring(0, 6)}******{cardNumberClean.Substring(12)}";

        // Log kaydı oluştur
        var log = HSMCommandLog.Create(
            device.Id,
            HSMCommandType.GeneratePINBlock,
            $"CardNumber:{maskedCard}",
            Guid.NewGuid().ToString(),
            maskedCard);

        try
        {
            // HSM'e gönder
            var hsmRequest = new GeneratePINBlockRequest
            {
                CardNumber = dto.CardNumber,
                PIN = dto.PIN,
                PINBlockFormat = dto.PINBlockFormat ?? "01",
                ZPKIndex = zpkKey.KeyIndex
            };

            var result = await _hsmService.GeneratePINBlockAsync(hsmRequest, cancellationToken);

            stopwatch.Stop();

            if (result.IsFailure)
            {
                log.SetResponse("", "99", false, (int)stopwatch.ElapsedMilliseconds, result.Error);
                await _logRepository.AddAsync(log, cancellationToken);
                await _logRepository.SaveChangesAsync(cancellationToken);

                return new GeneratePINBlockResponseDto
                {
                    IsSuccess = false,
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    ErrorMessage = result.Error
                };
            }

            var response = result.Value!;
            log.SetResponse(response.PINBlock, "00", true, (int)stopwatch.ElapsedMilliseconds);
            await _logRepository.AddAsync(log, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            device.RecordSuccessfulCommand();
            await _deviceRepository.UpdateAsync(device, cancellationToken);
            await _deviceRepository.SaveChangesAsync(cancellationToken);

            return new GeneratePINBlockResponseDto
            {
                IsSuccess = true,
                PINBlock = response.PINBlock,
                PINBlockFormat = response.PINBlockFormat,
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            log.SetResponse("", "99", false, (int)stopwatch.ElapsedMilliseconds, ex.Message);
            await _logRepository.AddAsync(log, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return new GeneratePINBlockResponseDto
            {
                IsSuccess = false,
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };
        }
    }
}