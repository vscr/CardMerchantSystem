using HSM.Application.DTOs;
using HSM.Domain.Entities;
using HSM.Domain.Enums;
using HSM.Domain.Repositories;
using HSM.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public class GenerateKeyCommandHandler
    : IRequestHandler<GenerateKeyCommand, Result<HSMKeyDto>>
{
    private readonly IHSMKeyRepository _keyRepository;
    private readonly IHSMDeviceRepository _deviceRepository;
    private readonly IHSMService _hsmService;

    public GenerateKeyCommandHandler(
        IHSMKeyRepository keyRepository,
        IHSMDeviceRepository deviceRepository,
        IHSMService hsmService)
    {
        _keyRepository = keyRepository;
        _deviceRepository = deviceRepository;
        _hsmService = hsmService;
    }

    public async Task<Result<HSMKeyDto>> Handle(
        GenerateKeyCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı isimde key var mı kontrol et
        var existing = await _keyRepository.GetByNameAsync(dto.KeyName, cancellationToken);
        if (existing != null)
            return Result.Failure<HSMKeyDto>("Bu isimde bir key zaten mevcut");

        // Key type bul
        var keyType = HSMKeyType.FromId<HSMKeyType>(dto.KeyTypeId);
        if (keyType == null)
            return Result.Failure<HSMKeyDto>("Geçersiz key tipi");

        // Device kontrol et
        var device = await _deviceRepository.GetByIdAsync(dto.HSMDeviceId, cancellationToken);
        if (device == null)
            return Result.Failure<HSMKeyDto>("HSM cihazı bulunamadı");

        // HSM'den key generate et
        var generateRequest = new GenerateKeyRequest
        {
            KeyType = keyType.Name,
            KeyLength = dto.KeyLength,
            KeyName = dto.KeyName
        };

        var generateResult = await _hsmService.GenerateKeyAsync(generateRequest, cancellationToken);
        if (generateResult.IsFailure)
            return Result.Failure<HSMKeyDto>(generateResult.Error!);

        var generatedKey = generateResult.Value!;

        // Key entity oluştur
        var keyResult = HSMKey.Create(
            dto.KeyName,
            keyType,
            generatedKey.KeyIndex,
            generatedKey.EncryptedKey,
            generatedKey.KeyCheckValue,
            dto.KeyLength,
            dto.HSMDeviceId,
            dto.ExpiryDate,
            dto.Description);

        if (keyResult.IsFailure)
            return Result.Failure<HSMKeyDto>(keyResult.Error!);

        var key = keyResult.Value!;

        await _keyRepository.AddAsync(key, cancellationToken);
        await _keyRepository.SaveChangesAsync(cancellationToken);

        return new HSMKeyDto
        {
            Id = key.Id,
            KeyName = key.KeyName,
            KeyType = key.KeyType.Name,
            KeyTypeDisplayName = key.KeyType.DisplayName,
            KeyIndex = key.KeyIndex,
            KeyCheckValue = key.KeyCheckValue!,
            KeyLength = key.KeyLength,
            IsActive = key.IsActive,
            ExpiryDate = key.ExpiryDate,
            Description = key.Description,
            HSMDeviceId = key.HSMDeviceId,
            HSMDeviceName = device.DeviceName,
            IsExpired = key.IsExpired,
            CreatedAt = key.CreatedAt
        };
    }
}